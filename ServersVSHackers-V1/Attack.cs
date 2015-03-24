using System;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Describes an attack between server and hacker.
    ///     Is created after successful attack to be written to DB.
    /// </summary>
    public sealed class Attack
    {
        private readonly Hacker _hacker;
        private readonly Server _server;
        private readonly DateTime _timeStamp;
        private readonly int _cashStolen;
        ///
        public Attack(Hacker hacker, Server server, DateTime timeStamp, int cashStolen)
        {
            _hacker = hacker;
            _server = server;
            _timeStamp = timeStamp;
            _cashStolen = cashStolen;

        }

        public int CashStolen
        {
            get { return _cashStolen; }
        }

        public Hacker Hacker
        {
            get { return _hacker; }
        }

        public Server Server
        {
            get { return _server; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
        }
    }
}