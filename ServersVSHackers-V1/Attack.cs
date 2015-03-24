using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    public sealed class Attack
    {
        private readonly Hacker _hacker;
        private readonly Server _server;
        private readonly DateTime _timeStamp;

        public Hacker Hacker { get { return _hacker; }
       
        }
        public Server Server { get { return _server; } }
        public DateTime TimeStamp { get { return _timeStamp; } }

        public Attack(Hacker hacker, Server server, DateTime timeStamp)
        {
            _hacker = hacker;
            _server = server;
            _timeStamp = timeStamp;
        }


    }
}
