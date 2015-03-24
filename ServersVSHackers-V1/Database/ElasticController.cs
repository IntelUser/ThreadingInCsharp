using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;


namespace ServersVSHackers_V1.Database
{
    class ElasticController : IDatabaseController
    {
        private readonly ElasticClient _client;
        private const string ATTACK_LOG_TABLE = "attack_logs";

        public ElasticController(IConnectionSettingsValues settings)
        {
            _client = new ElasticClient(settings);
            if (!CreateDatabase(ATTACK_LOG_TABLE))
            {
                Console.WriteLine("Index not created");
            }
        }

       


        public bool InsertBatch(IEnumerable<Attack> items)
        {

            var descriptor = new BulkDescriptor();
            descriptor.IndexMany<Attack>(items);
            descriptor.RequestConfiguration(r => r.MaxRetries(3));
            
            
           var result = _client.Bulk(descriptor);
            
            
            return result.IsValid;
        }

        public bool Insert(IEnumerable<Attack> attacks)
        {
            foreach (var atk in attacks)
            {
                _client.Index(atk, i =>
               i.Index((ATTACK_LOG_TABLE)));
            }
           

            return true;
        }

        public bool RemoveDatabase(string databaseName)
        {
            var response = _client.DeleteIndex(databaseName);
            return response.Acknowledged;
        }

        public IEnumerable<T> Search<T>(String databaseName, int limitResults = 10 ) where T : class
        {
            var result = _client.Search<T>(s => s
                .MatchAll()
                .Size(limitResults)
                .Index(databaseName));

            return result.Documents;
        }

        public bool CreateDatabase(string databasename)
        {
            if (_client.IndexExists(databasename).Exists)
            {
                Console.WriteLine(@"Warning: Index '{0}' already exists!", databasename);
            }
            var result = _client.CreateIndex(databasename);
            return result.Acknowledged;
        }
    }
}
