using System.Collections.Generic;
using System.Windows.Shapes;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Maintains a static collection with all drawn Polygons (countries)
    /// </summary>
    public static class Environment
    {
        public static List<Polygon> World = new List<Polygon>();
    }
}