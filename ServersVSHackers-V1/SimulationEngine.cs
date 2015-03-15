using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Nest;
using ServersVSHackers_V1.Database;

namespace ServersVSHackers_V1
{
    class SimulationEngine
    {
        
        ConcurrentBag<Hacker> ActiveHackers, BustedHackers = new ConcurrentBag<Hacker>();
        ConcurrentBag<Server> ActiveServers, HackedServers = new ConcurrentBag<Server>();
 

        private readonly IDatabaseController _dbController;
        //private Boolean is
        private const string ATTACK_LOG_TABLE = "attack_logs";

        public SimulationEngine()
        {

            // connect to local elastic database
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node, defaultIndex: ATTACK_LOG_TABLE);
            _dbController = new ElasticController(settings);
        }

        public void GenerateEntities()
        {
            ConcurrentBag<IEntity> cbEntities = new ConcurrentBag<IEntity>();
            //one MILLION entities
            int _numberOfEntities = 5000;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers
            //value of Slider
            int _balance = 6;


            //hacker code
            int _numberOfHackers = (_numberOfEntities * _balance) / 10;
            int _numberOfServers = (_numberOfEntities * (10 - _balance)) / 10;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers

            //generation code
            

            Parallel.For(0, _numberOfServers, i => cbEntities.Add(new Server(10, 10, new Country())));


            Parallel.For(0, _numberOfHackers, i => cbEntities.Add(new Hacker(10, 10)));


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
