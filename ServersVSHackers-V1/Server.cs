using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    public class Server : IEntity
    {
        public int Cash { get;  set; }
        public int ProtectionLevel { get; private set; }
        public Country Country { get; set; }
        public SimulationEngine.ValidPoint Coordinate { get; set; }
        bool IEntity.IsAlive { get; set; }

        public Server(int cashAmount, int protectionLevel)
        {
            Cash = cashAmount;
            ProtectionLevel = protectionLevel;
        }
    }
}
