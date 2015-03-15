using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    public class Country
    {
        public string Name { get; set; }

        public List<SimulationEngine.ValidPoint> validPoints;
        Random rnd = new Random();

        public Country(string name, List<SimulationEngine.ValidPoint> list)
        {
            Name = name;
            validPoints = list;
        }

        public SimulationEngine.ValidPoint GetValidPoint()
        {
            SimulationEngine.ValidPoint coordinate;
            while (true)
            {
                coordinate = validPoints[rnd.Next(validPoints.Count)];
                if (!coordinate.Used)
                {                    
                    coordinate.Used = true;
                    return coordinate;
                }
            }
        }

    }
   
}
