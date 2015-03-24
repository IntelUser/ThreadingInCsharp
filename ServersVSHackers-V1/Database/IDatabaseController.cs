using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1.Database
{
    interface IDatabaseController
    {
        /// <summary>
        /// Inserts an object into a database. 
        /// </summary>
        /// <typeparam name="Attack"> Represents the type of object that needs to be stored</typeparam>
        /// <param name="item">the object itself</param>
        /// <returns>the execution time in milliseconds</returns>
        bool InsertBatch(IEnumerable<Attack> items);
        /// <summary>
        /// Removes a complete database. Use with caution! Data is lost completely and cannot be recovered!
        /// </summary>
        /// <param name="databaseName">The name of the database that needs to be removed</param>
        /// <returns>True if deletion is succesfull or false when the action did not complete</returns>
        Boolean RemoveDatabase(String databaseName);

        Boolean CreateDatabase(String databaseName);

        /// <summary>
        /// Search a database
        /// </summary>
        /// <param name="searchPhrase">A string that mathes the search</param>
        /// <returns>An Ienumerable with mathing results</returns>
        IEnumerable<T> Search<T>(String databaseName, int limitResults = 10) where T : class;

        bool Insert(IEnumerable<Attack> attacks);
    }
}
