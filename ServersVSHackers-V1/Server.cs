namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Describes a server, implements IEntity interface.
    /// </summary>
    internal class Server : IEntity
    {
        public Server(int cashAmount, int protectionLevel)
        {
            Cash = cashAmount;
            ProtectionLevel = protectionLevel;
        }

        public int ProtectionLevel { get; private set; }
        public int Cash { get; set; }
        public Country Country { get; set; }
        public SimulationEngine.ValidPoint Coordinate { get; set; }
        bool IEntity.IsAlive { get; set; }
    }
}