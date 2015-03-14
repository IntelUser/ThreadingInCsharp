using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServersVSHackers_V1.Database
{
    class ElasticController : IDatabaseController
    {
        
        public ElasticController()
        {
            var node = new Uri("http://localhost:9200");

            var settings = new ConnectionSettings(
                node,
                defaultIndex: "my-application"
            );

            var client = new ElasticClient(settings);
        }

        public long Insert<T>(T item)
        {
            
        }

        public bool RemoveDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Search<T>(string searchPhrase)
        {
            throw new NotImplementedException();
        }
    }
}
