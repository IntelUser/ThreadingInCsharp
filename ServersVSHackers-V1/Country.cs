using System;
using System.Collections.Generic;
using System.Windows;
using TestWW3;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Describes a country to be assigned to a hacker or server
    ///     Based on drawn Polygons, contains a list with all points in this Polygon
    /// </summary>
    public class Country
    {
        public Queue<SimulationEngine.ValidPoint> ValidPointsQueue;
        public List<SimulationEngine.ValidPoint> validPoints;
        public Country(string name, List<SimulationEngine.ValidPoint> list)
        {
            Name = name;
            validPoints = list;
            validPoints.Shuffle();
            ValidPointsQueue = new Queue<SimulationEngine.ValidPoint>(validPoints);
        }

        public string Name { get; set; }

        /// <summary>
        /// Returns a ValidPoint (xy coordinate) from internal list.
        /// </summary>
        /// <returns>ValidPoint</returns>
        public SimulationEngine.ValidPoint GetValidPoint()
        {           
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