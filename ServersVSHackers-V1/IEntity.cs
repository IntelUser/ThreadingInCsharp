using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    public interface IEntity
    {
        bool IsAlive { get; set; }
        Country Country { get; set; }
        int Cash { get; set; }
        SimulationEngine.ValidPoint Coordinate { get; set; }


    }
}
