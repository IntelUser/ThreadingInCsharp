namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Describes a hacker, implements IEntity interface.
    /// </summary>
    public class Hacker : IEntity
    {
        public Hacker(int id, int skillLevel)
        {
            Id = id;
            SkillLevel = skillLevel;
            Cash = 0;
            IsAlive = true;
        }

        public int Id { get; private set; }
        public int SkillLevel { get; private set; }
        public bool IsAlive { get; set; }
        public int Cash { get; set; }


        public Country Country { get; set; }
        public SimulationEngine.ValidPoint Coordinate { get; set; }

        /// <summary>
        /// Transfers money owner by server to hacker
        /// </summary>
        /// <param name="server"></param>
        public void StealCash(Server server)
        {
            Cash += server.Cash;
        }

        /// <summary>
        /// Unused. For future development.
        /// </summary>
        public void IncreaseSkillLevel()
        {
            if (SkillLevel <= 9)
            {
                SkillLevel++;
            }
        }
    }
}