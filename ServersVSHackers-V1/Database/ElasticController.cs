using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1.Database
{
    class ElasticController : IDatabaseController
    {
        public long Insert<T>(T item)
        {
            throw new NotImplementedException();
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
