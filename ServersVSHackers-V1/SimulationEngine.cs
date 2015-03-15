using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ServersVSHackers_V1
{
    public class SimulationEngine
    {
        
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

        public void GenerateEntities()
        {
            //one MILLION entities
            int _numberOfEntities = Convert.ToInt32(Properties.Settings.Default.numberOfEntities);
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers
            //value of Slider
            int _balance = Convert.ToInt32(Properties.Settings.Default.balanceValue);


            //hacker code
            int _numberOfHackers = (_numberOfEntities * _balance) / 10;
            int _numberOfServers = (_numberOfEntities * (10 - _balance)) / 10;
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
                                                                                Generator.GetRandomNumber(2, 200))));


            Parallel.For(0, _numberOfHackers, i => ActiveHackers.Add(new Hacker(
                                                                                Generator.GetRandomNumber(100, 10000),
                                                                                Generator.GetRandomNumber(1, 100))));
            
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

        private void StartAttacks()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //moet een hacker en een server pakken en dan bepalen wie wint. 
                //bij verlies verplaatsen naar andere bag.
                //eerst maar eens lijntjes trekken..
                IEntity attacker;
                IEntity defender;
                DoubleAnimation lineFade = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000));

                for (int i = 0; i < 100; i++)
                {
                    bool succesAttacker = ActiveHackers.TryPeek(out attacker);
                    bool succesDefender = ActiveServers.TryPeek(out defender);
                    Line l = new Line
                    {
                        //starting point
                        X1 = attacker.Coordinate.X,
                        Y1 = attacker.Coordinate.Y,
                        //end point
                        X2 = defender.Coordinate.X,
                        Y2 = defender.Coordinate.Y,
                        Stroke = Brushes.Orange,
                        StrokeThickness = 6
                    };
                    mainWindow.WorldCanvas.Children.Add(l);
                    l.BeginAnimation(UIElement.OpacityProperty, lineFade);
                    //Thread.SpinWait(200);
                }

            }));

        }

        private void GenerateCountries()
        {
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
        }

        private void PlaceEntitiesOnWorld()
        {
        

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                foreach (var hacker in ActiveHackers)
                {
                    
                    Rectangle rect;
                    rect = new Rectangle();
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
