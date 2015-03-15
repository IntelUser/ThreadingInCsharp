using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWW3
{
    class Server : IEntity
    {
        private int Hp { get; set; }
        public Server()
        {
            Hp = 10;
        }
    }
}
