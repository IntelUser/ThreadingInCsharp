using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    class Country
    {
        string Name { get; set; }
        private List<SimulationEngine.ValidPoint> validPoints;
        
        public Country(string name, List<SimulationEngine.ValidPoint> list)
        {
            Name = name;
            validPoints = list;
        }

    }
   
}
