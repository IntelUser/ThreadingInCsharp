using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    class Server : ICharacter
    {
        private bool _isAlive;
        private int _cash, _protectionLevel;
        private Country _country;



        public Server(int cashAmount, int protectionLevel, Country country)
        {
            _cash = cashAmount;
            _protectionLevel = protectionLevel;
            _country = country;
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
            _cash += amount;
        }

        public void SetDead()
        {
            throw new NotImplementedException();
        }
    }
}
