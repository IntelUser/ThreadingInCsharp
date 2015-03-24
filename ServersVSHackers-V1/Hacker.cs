using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ServersVSHackers_V1
{
    public class Hacker : IEntity
    {
        public int Id { get; private set; }
        public bool IsAlive { get; set; }
        public int Cash {get; set; }
        public int SkillLevel { get; private set; }


        public Country Country { get; set; }
        public SimulationEngine.ValidPoint Coordinate { get; set; }
        public Hacker(int id, int skillLevel)
        {
            Id = id;
            SkillLevel = skillLevel;
            Cash = 0;
            IsAlive = true;
        }
        public void StealCash(Server server)
        {
            this.Cash += server.Cash;
        }
   
        public void IncreaseSkillLevel()
        {
            if(SkillLevel <= 9)
            {
                SkillLevel++;
            }
        }
    }
}
