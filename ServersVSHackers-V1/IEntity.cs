namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Describes what entities Hacker and Server need to implement.
    /// </summary>
    public interface IEntity
    {
        bool IsAlive { get; set; }
        Country Country { get; set; }
        int Cash { get; set; }
        SimulationEngine.ValidPoint Coordinate { get; set; }
    }
}