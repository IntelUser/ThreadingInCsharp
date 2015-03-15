using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    class Server : IEntity
    {
        private bool _isAlive;
        public int Cash { get; set; }
        public int ProtectionLevel { get; private set; }
        private SimulationEngine.ValidPoint coordinate;

        public Country country { get; set; }


        public Server(int cashAmount, int protectionLevel)
        {
            Cash = cashAmount;
            ProtectionLevel = protectionLevel;
        }

        public void Action()
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        public void UpdateCashAmount(int amount)
        {
            Cash += amount;
        }

        public void SetDead()
        {
            throw new NotImplementedException();
        }

        public Country C
        {
            get { return country; }
            set { country = value; }
        }

        public SimulationEngine.ValidPoint Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }

        
    }
}
