using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Nest;
using ServersVSHackers_V1.Database;
using TestWW3;

namespace ServersVSHackers_V1
{
    public class SimulationEngine
    {
        private readonly ManualResetEvent _syncEvent = new ManualResetEvent(false);
        private readonly List<Country> _countryList = new List<Country>();
       
        private readonly MainWindow _mainWindow;
       
        private const int BATCH_SIZE = 1000;
        public ConcurrentQueue<Attack> _attacks = new ConcurrentQueue<Attack>();
       


        public int NumberOfHackers, NumberOfServers;

        public ConcurrentQueue<IEntity> ActiveHackers, ActiveServers; 
        public ConcurrentBag<IEntity> HackedServers, BustedHackers;
         
        

        public SimulationEngine(MainWindow window)
        {
            _mainWindow = window;
          

            ActiveHackers = new ConcurrentQueue<IEntity>();
            BustedHackers = new ConcurrentBag<IEntity>();
            ActiveServers = new ConcurrentQueue<IEntity>();
            HackedServers = new ConcurrentBag<IEntity>();
            

        }

        private void SetStatus(string text, Brush brush)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.StatusTextBlock.Text = text;
                _mainWindow.StatusTextBlock.Background = brush ?? Brushes.Green;
            }));
            
        }
        public void GenerateEntities()
        {
            SetStatus("Generating entities", Brushes.Red);
            //one MILLION entities
            var numberOfEntities = Convert.ToInt32(Properties.Settings.Default.numberOfEntities);
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers
            //value of Slider
            var balance = Convert.ToInt32(Properties.Settings.Default.balanceValue);


            //hacker code
            NumberOfHackers = (numberOfEntities * balance) / 10;
            NumberOfServers = (numberOfEntities * (10 - balance)) / 10;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers

            //generation code
            _mainWindow.Log(String.Format("Hackers created: {0}, servers created: {1}", NumberOfHackers, NumberOfServers) );
            _mainWindow.ServerAmountTextBlock.Text = NumberOfServers.ToString();
            _mainWindow.HackerAmountTextBlock.Text = NumberOfHackers.ToString();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, NumberOfServers, i => ActiveServers.Enqueue(new Server(
                                                                                Generator.GetRandomNumber(100, 10000),
                                                                                Generator.GetRandomNumber(1, 10))));


            Parallel.For(0, NumberOfHackers, i => ActiveHackers.Enqueue(new Hacker(
                                                                                Generator.GetRandomNumber(100, 10000),
                                                                                Generator.GetRandomNumber(1, 10))));

            sw.Stop();
            _mainWindow.Log("Generation took: " + sw.ElapsedMilliseconds);
            GenerateCountries();
            AssignCountries();
            //dit duurt extreem lang...
            var assignCoordinatesThread = new Thread(AssignCoordinates);
            assignCoordinatesThread.Start();
            //dit waarschijnlijk nog veel langer
            var placeEntitiesOnWorldThread = new Thread(PlaceEntitiesOnWorld);
            placeEntitiesOnWorldThread.Start();

            //Thread startAttacksThread = new Thread(StartAttacks);
            //startAttacksThread.Start(); 
        }

        public void PerformAttack(Brush b)
        {
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //moet een hacker en een server pakken en dan bepalen wie wint. 
                //bij verlies verplaatsen naar andere bag.
                //eerst maar eens lijntjes trekken..
                IEntity attacker;
                IEntity defender;
                
                var lineFade = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));

                var succesAttacker = ActiveHackers.TryDequeue(out attacker);
                var succesDefender = ActiveServers.TryDequeue(out defender);

                if (!succesDefender || !succesAttacker)
                {
                    

                    if (ActiveHackers.IsEmpty)
                    {
                        //DoSomething
                        var totalServerCash = (from entity in ActiveServers select entity.Cash).Sum();

                        _mainWindow.ServersWin(totalServerCash);
                        return;
                    }
                    else if (ActiveServers.IsEmpty)
                    {
                        //DoSomethingElse
                        var totalHackerCash = (from entity in ActiveHackers select entity.Cash).Sum();
                        
                        _mainWindow.HackersWin(totalHackerCash);
                        return;
                    }
                    else
                    {
                        return;    
                    }                   
                    //set global flag
                    //TODO  
                }
                    //simple attack
                    #region attack
                    if (succesAttacker && succesDefender)
                    {
                        var hacker = (Hacker)attacker;
                        var server = (Server)defender;
                        var stolenCash = 0;

                        var winner = DecideWinner(hacker, server);
                        SetWinner(winner);

                        _mainWindow.ServerAmountTextBlock.Text = NumberOfServers.ToString(); // redraw number of servers
                        _mainWindow.HackerAmountTextBlock.Text = NumberOfHackers.ToString(); // redraw number of hackers

                        // hacker wins
                        if (winner.GetType() == typeof (Hacker))
                        {
                            hacker.StealCash(server);
                            stolenCash = server.Cash;
                            RemoveUiElement(server.Coordinate);
                        }
                        else
                        {
                            RemoveUiElement(hacker.Coordinate);
                        }

                        StoreAttack(new Attack(hacker, server, DateTime.Now, stolenCash));

                        #endregion
                    }

                    var line = new Line
                    {
                        //starting point
                        X1 = attacker.Coordinate.X,
                        Y1 = attacker.Coordinate.Y,
                        //end point
                        X2 = defender.Coordinate.X,
                        Y2 = defender.Coordinate.Y,
                        Stroke = b,
                        StrokeThickness = 6
                    };
                    _mainWindow.WorldCanvas.Children.Add(line);
                    line.BeginAnimation(UIElement.OpacityProperty, lineFade);              

            }));

        }

        /// <summary>
        /// Decide which entity is the winner.
        /// </summary>
        /// <param name="hacker"></param>
        /// <param name="server"></param>
        /// <returns>The entity of the winner</returns>
        private static IEntity DecideWinner (Hacker hacker, Server server)
        {
            if (hacker.SkillLevel > server.ProtectionLevel)
            {
               
                return hacker;
            }
            if (hacker.SkillLevel.Equals(server.ProtectionLevel))
            {
                if(Generator.GetRandomNumber(0, 10) < 5)
                {
                    return server;
                }
                return hacker;
            }
            return server;
        }

        private void SetWinner(IEntity entity)
        {
            if (entity.GetType() == typeof (Hacker))
            {
                HackedServers.Add(entity);      // add server to hackedlist  
                ActiveHackers.Enqueue(entity);      // put the hacker back in the list
                NumberOfServers--;              // decrease number of servers
            }
            else if (entity.GetType() == typeof (Server))
            {
                BustedHackers.Add(entity);          // add hacker to bustlist
                ActiveServers.Enqueue(entity);          // put the server back in the list
                NumberOfHackers--;                  // decrease hackers
            }
        }

        private void RemoveUiElement(ValidPoint point)
        {
            foreach (var element in _mainWindow.WorldCanvas.Children.Cast<UIElement>().Where(ui => ui.Uid.Equals(String.Format("{0}{1}", point.X, point.Y))).ToList())
            {
                _mainWindow.WorldCanvas.Children.Remove(element);
            }
                
        }

        private void GenerateCountries()
        {
            SetStatus("Generating countries.", Brushes.Red);

            foreach (var polygon in Environment.World)
            {
                _countryList.Add(new Country(polygon.Name, DefinePixelsinCountries(polygon)));
                
            }
            foreach (var c in _countryList)
            {
                _mainWindow.Log(String.Format("Amount of valid points in {0} are: {1}.", c.Name, c.validPoints.Count));
            }
        }

        private void AssignCountries()
        {
            SetStatus("Assigning countries.", Brushes.Red);

            //ergens anders? : 
            var rnd = new Random();
            Parallel.ForEach(ActiveHackers, currentHacker =>
            {
                currentHacker.Country = _countryList[rnd.Next(_countryList.Count)];
            });
            
            Parallel.ForEach(ActiveServers, currentServer =>
            {
                currentServer.Country = _countryList[rnd.Next(_countryList.Count)];
            });
        }

        private void AssignCoordinates()
        {
            SetStatus("Assigning coordinates.", Brushes.Red);

            foreach (var hacker in ActiveHackers)
            {
                
                hacker.Coordinate = hacker.Country.GetValidPoint();
                //Console.WriteLine("hacker x= {0}, y= {1}", hacker.Coordinate.X, hacker.Coordinate.Y);
            }
            foreach (var server in ActiveServers)
            {
                server.Coordinate = server.Country.GetValidPoint();
                //Console.WriteLine("server x= {0}, y= {1}", server.Coordinate.X, server.Coordinate.Y);
            }
            _syncEvent.Set();

        }

        private void PlaceEntitiesOnWorld()
        {

            _syncEvent.WaitOne();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SetStatus("Placing entities.", Brushes.Red);
                foreach (var hacker in ActiveHackers)
                {
                    var rect = new Rectangle
                    {
                        Uid = String.Format(hacker.Coordinate.X + hacker.Coordinate.Y.ToString()),
                        Stroke = new SolidColorBrush(Colors.Red),
                        Fill = new SolidColorBrush(Colors.Red),
                        Width = 2,
                        Height = 2
                    };
                    Canvas.SetLeft(rect, hacker.Coordinate.X);
                    Canvas.SetTop(rect, hacker.Coordinate.Y);
                    _mainWindow.WorldCanvas.Children.Add(rect);
                    //Canvasx.Refresh();
                    //tt.Stop();
                    //MessageBox.Show(tt.ElapsedTicks.ToString());
                }
                foreach (var server in ActiveServers)
                {
                    var rect = new Rectangle
                    {
                        Uid = String.Format(server.Coordinate.X + server.Coordinate.Y.ToString()),
                        Stroke = new SolidColorBrush(Colors.Blue),
                        Fill = new SolidColorBrush(Colors.Blue),
                        Width = 2,
                        Height = 2
                    };

                    Canvas.SetLeft(rect, server.Coordinate.X);
                    Canvas.SetTop(rect, server.Coordinate.Y);
                    _mainWindow.WorldCanvas.Children.Add(rect);
                    //Canvasx.Refresh();
                    //tt.Stop();
                    //MessageBox.Show(tt.ElapsedTicks.ToString());
                }
                SetStatus("Ready...", null);

            }));
        }

        public List<ValidPoint> DefinePixelsinCountries(Polygon poly)
        {
            var points = new List<ValidPoint>();
            

            var ps = poly.Points.ToArray();

            for (double y = 0; y < _mainWindow.WorldCanvas.Height; y++)
            {
                for (double x = 0; x < _mainWindow.WorldCanvas.Width; x++)
                {
                    if (IsPointInPolygon(ps, new Point(x, y)))
                    {
                        points.Add(new ValidPoint() {X = x, Y = y, Used = false});
                    }

                }
            }
            return points;
        }

        private static bool IsPointInPolygon(Point[] polygon, Point point)
        {
            var isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
       
        public struct ValidPoint
        {
            public double X;
            public double Y;
            public bool Used;
        }

       

        private void StoreAttack(Attack attack)
        {
            _attacks.Enqueue(attack);
                
            
           
        }
    }
}
