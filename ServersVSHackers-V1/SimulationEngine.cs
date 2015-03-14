using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using ServersVSHackers_V1.Database;

namespace ServersVSHackers_V1
{
    class SimulationEngine
    {

        private readonly IDatabaseController _dbController;
        private const string ATTACK_LOG_TABLE = "attack_logs";

        public SimulationEngine()
        {
            // connect to local elastic database
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node, defaultIndex: "my-application");
            _dbController = new ElasticController(settings);
        }

        private void InitDatabase()
        {
            _dbController.CreateDatabase(ATTACK_LOG_TABLE);
        }







    }
}
