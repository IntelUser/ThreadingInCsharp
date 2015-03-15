using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    public interface IEntity
    {
        /// <summary>
        /// Call action to engage all characters action.
        /// </summary>
        void Action();

        /// <summary>
        /// Check if the Character is still alive
        /// </summary>
        /// <returns></returns>
        bool IsAlive();

        /// <summary>
        /// Increases or decreased cash for character.
        /// </summary>
        /// <param name="amount"></param>
        void UpdateCashAmount(int amount);

        void SetDead();

        Country C{ get; set; }

        SimulationEngine.ValidPoint Coordinate {get; set;}

    }
}
