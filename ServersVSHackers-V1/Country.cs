using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using TestWW3;

namespace ServersVSHackers_V1
{
    public class Country
    {
        public string Name { get; set; }

        public List<SimulationEngine.ValidPoint> validPoints;
        public Queue<SimulationEngine.ValidPoint> ValidPointsQueue; 
        //Random rnd = new Random();

        public Country(string name, List<SimulationEngine.ValidPoint> list)
        {
            Name = name;
            validPoints = list;
            validPoints.Shuffle();
            ValidPointsQueue = new Queue<SimulationEngine.ValidPoint>(validPoints);

        }

        public SimulationEngine.ValidPoint GetValidPoint()
        {
            /*
             * Below random code is probably no longer necessary
             * because of Shuffle() extension method.
             * 
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
            */

            try
            {
                return ValidPointsQueue.Dequeue();
            }
            catch (InvalidOperationException ex)
            {
                Application.Current.Shutdown();
                return new SimulationEngine.ValidPoint();
            }

        }

    }
   
}
