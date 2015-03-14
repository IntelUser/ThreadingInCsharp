using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    class SimulationEngine
    {
        private readonly int _numberOfEntities = Properties.Settings.Default.numberOfEntities;
        private ConcurrentBag<ICharacter> _entities = new ConcurrentBag<ICharacter>();
  
        public void CreateEntities()
        {
            Parallel.For(0, _numberOfEntities, i =>
            {

            });
        }


    }
}
