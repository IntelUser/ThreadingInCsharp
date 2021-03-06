﻿using System;
using System.CodeDom;
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
        ManualResetEvent syncEvent = new ManualResetEvent(false);
        public int _numberOfHackers, _numberOfServers;
        public ConcurrentBag<IEntity> ActiveHackers, BustedHackers;
        public ConcurrentBag<IEntity> ActiveServers, HackedServers;
        List<Country> CountryList = new List<Country>(); 

        private readonly IDatabaseController _dbController;
        //private Boolean is
        private const string ATTACK_LOG_TABLE = "attack_logs";
        private MainWindow mainWindow;
        public SimulationEngine(MainWindow window)
        {
            mainWindow = window;
            // connect to local elastic database
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node, defaultIndex: ATTACK_LOG_TABLE);
            _dbController = new ElasticController(settings);

           ActiveHackers = new ConcurrentBag<IEntity>();
           BustedHackers = new ConcurrentBag<IEntity>();
           ActiveServers = new ConcurrentBag<IEntity>();
           HackedServers = new ConcurrentBag<IEntity>();

        }

        private void SetStatus(string text, Brush brush)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                mainWindow.StatusTextBlock.Text = text;
                if (brush == null)
            {
                mainWindow.StatusTextBlock.Background = Brushes.Green;
            }
            else
            {
                mainWindow.StatusTextBlock.Background = brush;
            }
            }));
            
        }
        public void GenerateEntities()
        {
            SetStatus("Generating entities", Brushes.Red);
            //one MILLION entities
            int _numberOfEntities = Convert.ToInt32(Properties.Settings.Default.numberOfEntities);
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers
            //value of Slider
            int _balance = Convert.ToInt32(Properties.Settings.Default.balanceValue);


            //hacker code
            _numberOfHackers = (_numberOfEntities * _balance) / 10;
            _numberOfServers = (_numberOfEntities * (10 - _balance)) / 10;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers

            //generation code
            mainWindow.Log(String.Format("Hackers created: {0}, servers created: {1}", _numberOfHackers, _numberOfServers) );
            mainWindow.ServerAmountTextBlock.Text = _numberOfServers.ToString();
            mainWindow.HackerAmountTextBlock.Text = _numberOfHackers.ToString();
            Stopwatch t = new Stopwatch();
            t.Start();
            Parallel.For(0, _numberOfServers, i => ActiveServers.Add(new Server(
                                                                                Generator.GetRandomNumber(100, 10000),
                                                                                Generator.GetRandomNumber(1, 10))));


            Parallel.For(0, _numberOfHackers, i => ActiveHackers.Add(new Hacker(
                                                                                Generator.GetRandomNumber(100, 10000),
                                                                                Generator.GetRandomNumber(1, 10))));

            t.Stop();
            mainWindow.Log("Generation took: " + t.ElapsedMilliseconds);
            GenerateCountries();
            AssignCountries();
            //dit duurt extreem lang...
            Thread AssignCoordinatesThread = new Thread(AssignCoordinates);
            AssignCoordinatesThread.Start();
            //dit waarschijnlijk nog veel langer
            Thread PlaceEntitiesOnWorldThread = new Thread(PlaceEntitiesOnWorld);
            PlaceEntitiesOnWorldThread.Start();

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
                
                DoubleAnimation lineFade = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));

                    bool succesAttacker = ActiveHackers.TryTake(out attacker);
                    bool succesDefender = ActiveServers.TryTake(out defender);
                if (!succesDefender || !succesAttacker)
                {
                    if (ActiveHackers.IsEmpty)
                    {
                        //DoSomething
                        int c = (from IEntity in ActiveServers select IEntity.Cash).Sum();

                        mainWindow.ServersWin(c);
                    }
                    else if (ActiveServers.IsEmpty)
                    {
                        //DoSomethingElse
                        int c = (from IEntity in ActiveHackers select IEntity.Cash).Sum();
                        
                        mainWindow.HackersWin(c);
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
                        Hacker hacker = (Hacker)attacker;
                        Server server = (Server)defender;
                        List<UIElement> itemstoremove = new List<UIElement>();
                        if (hacker.SkillLevel > server.ProtectionLevel)
                        {
                            //server dood
                            string toDelete =
                                    String.Format(server.Coordinate.X.ToString() + server.Coordinate.Y.ToString());
                            HackedServers.Add(server);                            
                            hacker.UpdateCashAmount(server.Cash);
                            ActiveHackers.Add(hacker);
                            _numberOfServers--;
                            
                            mainWindow.ServerAmountTextBlock.Text = _numberOfServers.ToString();
                            mainWindow.Log(String.Format("Hacker {0} from {1} won! He gained {2} in cash! Server is dead...", hacker.SkillLevel, hacker.C.Name, server.Cash));
                            //remove server dot
                            foreach (UIElement ui in mainWindow.WorldCanvas.Children)
                            {
                                if (
                                    ui.Uid.Equals(toDelete))
                                {
                                    itemstoremove.Add(ui);
                                }
                            }
                            foreach (UIElement ui in itemstoremove)
                            {
                                mainWindow.WorldCanvas.Children.Remove(ui);
                            }                        
                        }
                        else if (hacker.SkillLevel < server.ProtectionLevel)
                        {
                            //hacker dood
                            string toDelete =
                                    String.Format(hacker.Coordinate.X.ToString() + hacker.Coordinate.Y.ToString());
                            hacker.SetDead();
                            BustedHackers.Add(hacker);
                            ActiveServers.Add(server);
                            _numberOfHackers--;
                            
                            
                            mainWindow.HackerAmountTextBlock.Text = _numberOfHackers.ToString();
                            mainWindow.Log(String.Format("Server {0} from {1} won! We must implement what he gained! Hacker is busted...", server.Coordinate.X, server.C.Name));
                            //remove hacker dot
                            foreach (UIElement ui in mainWindow.WorldCanvas.Children)
                            {
                                if (ui.Uid.Equals(toDelete))
                                    
                                {
                                    itemstoremove.Add(ui);
                                }
                            }
                            foreach (UIElement ui in itemstoremove)
                            {
                                mainWindow.WorldCanvas.Children.Remove(ui);
                            }
                        }
                        else if (hacker.SkillLevel == server.ProtectionLevel)
                        {
                            //we are EVEN! faith decides who wins...
                            int faith = Generator.GetRandomNumber(0,10);
                            if (faith < 5)
                            {
                                //server dood
                                //hoop dubbele code
                                //TODO
                                string toDelete =
                                    String.Format(server.Coordinate.X.ToString() + server.Coordinate.Y.ToString());
                                HackedServers.Add(server);
                                hacker.UpdateCashAmount(server.Cash);
                                ActiveHackers.Add(hacker);
                                _numberOfServers--;
                                mainWindow.ServerAmountTextBlock.Text = _numberOfServers.ToString();
                                mainWindow.Log(
                                String.Format("Hacker {0} from {1} won! He gained {2} in cash! Server is dead...",
                                        hacker.SkillLevel, hacker.C.Name, server.Cash));

                                
                                foreach (UIElement ui in mainWindow.WorldCanvas.Children)
                                {
                                    if (ui.Uid.Equals(toDelete))
                                    {
                                        itemstoremove.Add(ui);
                                    }
                                }
                                foreach (UIElement ui in itemstoremove)
                                {
                                    mainWindow.WorldCanvas.Children.Remove(ui);
                                }
                            }
                            else
                            {
                                //hacker dood
                                //hoop dubbele code
                                //TODO
                                string toDelete =
                                    String.Format(hacker.Coordinate.X.ToString() + hacker.Coordinate.Y.ToString());
                                hacker.SetDead();
                                BustedHackers.Add(hacker);
                                ActiveServers.Add(server);
                                _numberOfHackers--;
                                mainWindow.HackerAmountTextBlock.Text = _numberOfHackers.ToString();
                                mainWindow.Log(String.Format("Server {0} from {1} won! We must implement what he gained! Hacker is busted...", server.Coordinate.X, server.C.Name));
                                //remove hacker dot
                                
                                
                                
                                foreach (UIElement ui in mainWindow.WorldCanvas.Children)
                                {
                                    if (ui.Uid.Equals(toDelete))
                                    {
                                        itemstoremove.Add(ui);
                                    }
                                }
                                foreach (UIElement ui in itemstoremove)
                                {
                                    mainWindow.WorldCanvas.Children.Remove(ui);
                                }                            
                            }


                        }
                        
                    #endregion
                    }
                    Line l = new Line
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
                    mainWindow.WorldCanvas.Children.Add(l);
                    l.BeginAnimation(UIElement.OpacityProperty, lineFade);              

            }));

        }

        private void GenerateCountries()
        {
            SetStatus("Generating countries.", Brushes.Red);

            foreach (var polygon in Environment.World)
            {
                CountryList.Add(new Country(polygon.Name, DefinePixelsinCountries(polygon)));
                
            }
            foreach (var c in CountryList)
            {
                mainWindow.Log(String.Format("Amount of valid points in {0} are: {1}.", c.Name, c.validPoints.Count));
            }
        }

        private void AssignCountries()
        {
            SetStatus("Assigning countries.", Brushes.Red);

            //ergens anders? : 
            Random rnd = new Random();
            Parallel.ForEach(ActiveHackers, currentHacker =>
            {
                currentHacker.C = CountryList[rnd.Next(CountryList.Count)];
            });
            
            Parallel.ForEach(ActiveServers, currentServer =>
            {
                currentServer.C = CountryList[rnd.Next(CountryList.Count)];
            });
        }

        private void AssignCoordinates()
        {
            SetStatus("Assigning coordinates.", Brushes.Red);

            foreach (var hacker in ActiveHackers)
            {
                
                hacker.Coordinate = hacker.C.GetValidPoint();
                Console.WriteLine("hacker x= {0}, y= {1}", hacker.Coordinate.X, hacker.Coordinate.Y);
            }
            foreach (var server in ActiveServers)
            {

                server.Coordinate = server.C.GetValidPoint();
                Console.WriteLine("server x= {0}, y= {1}", server.Coordinate.X, server.Coordinate.Y);
            }
            syncEvent.Set();

        }

        private void PlaceEntitiesOnWorld()
        {

            syncEvent.WaitOne();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SetStatus("Placing entities.", Brushes.Red);
                foreach (var hacker in ActiveHackers)
                {
                    
                    Rectangle rect;
                    rect = new Rectangle();
                    rect.Uid = String.Format(hacker.Coordinate.X.ToString() + hacker.Coordinate.Y.ToString());
                    rect.Stroke = new SolidColorBrush(Colors.Red);
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    rect.Width = 2;
                    rect.Height = 2;
                    Canvas.SetLeft(rect, hacker.Coordinate.X);
                    Canvas.SetTop(rect, hacker.Coordinate.Y);
                    mainWindow.WorldCanvas.Children.Add(rect);
                    //Canvasx.Refresh();
                    //tt.Stop();
                    //MessageBox.Show(tt.ElapsedTicks.ToString());
                }
                foreach (var server in ActiveServers)
                {

                    Rectangle rect;
                    rect = new Rectangle();
                    rect.Uid = String.Format(server.Coordinate.X.ToString() + server.Coordinate.Y.ToString());                    
                    rect.Stroke = new SolidColorBrush(Colors.Blue);
                    rect.Fill = new SolidColorBrush(Colors.Blue);
                    rect.Width = 2;
                    rect.Height = 2;
                    Canvas.SetLeft(rect, server.Coordinate.X);
                    Canvas.SetTop(rect, server.Coordinate.Y);
                    mainWindow.WorldCanvas.Children.Add(rect);
                    //Canvasx.Refresh();
                    //tt.Stop();
                    //MessageBox.Show(tt.ElapsedTicks.ToString());
                }
                SetStatus("Ready...", null);

            }));
        }

        public List<ValidPoint> DefinePixelsinCountries(Polygon poly)
        {
            List<ValidPoint> points = new List<ValidPoint>();
            

            Point[] ps = poly.Points.ToArray();

            for (double y = 0; y < mainWindow.WorldCanvas.Height; y++)
            {
                for (double x = 0; x < mainWindow.WorldCanvas.Width; x++)
                {
                    if (IsPointInPolygon(ps, new Point(x, y)))
                    {
                        points.Add(new ValidPoint() {X = x, Y = y, Used = false});
                    }

                }
            }
            return points;
        }

        private bool IsPointInPolygon(Point[] polygon, Point point)
        {
            bool isInside = false;
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

        private void InitDatabase()
        {
            _dbController.CreateDatabase(ATTACK_LOG_TABLE);
        }

        private void StoreAttacks(IEnumerable<Attack> attacks )
        {
            _dbController.Insert(attacks);
        }






    }
}
