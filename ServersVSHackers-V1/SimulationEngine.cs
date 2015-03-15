using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Nest;
using ServersVSHackers_V1.Database;

namespace ServersVSHackers_V1
{
    public class SimulationEngine
    {
        
        ConcurrentBag<Hacker> ActiveHackers, BustedHackers = new ConcurrentBag<Hacker>();
        ConcurrentBag<Server> ActiveServers, HackedServers = new ConcurrentBag<Server>();
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
        }

        public void GenerateEntities()
        {
            ConcurrentBag<IEntity> cbEntities = new ConcurrentBag<IEntity>();
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
            Stopwatch t = new Stopwatch();
            t.Start();
            Parallel.For(0, _numberOfServers, i => cbEntities.Add(new Server(10, 10)));
            Parallel.For(0, _numberOfHackers, i => cbEntities.Add(new Hacker(10, 10)));
            
            t.Stop();
            mainWindow.Log("Generation took: " + t.ElapsedMilliseconds);
            GenerateCountries();
        }


        private void GenerateCountries()
        {
            foreach (var polygon in Environment.World)
            {
                CountryList.Add(new Country(polygon.Name, DefinePixelsinCountries(polygon)));
            }
        }

        public List<ValidPoint> DefinePixelsinCountries(Polygon poly)
        {
            List<ValidPoint> points = new List<ValidPoint>();
            

            Point[] ps = poly.Points.ToArray();

            for (double y = 0; y < poly.Height; y++)
            {
                for (double x = 0; x < poly.Width; x++)
                {
                    if (IsPointInPolygon(ps, new Point(x, y)))
                    {
                        points.Add(new ValidPoint() {x = x, y = y});
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
            public double x;
            public double y;
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
