﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersVSHackers_V1
{
    class Hacker : IEntity
    {
        private readonly int _id;
        private bool _isAlive;
        private int _skillLevel, _cash;
        public int Cash { get { return _cash; }
           set { }
        }
        public int SkillLevel { get { return _skillLevel; } }
        private SimulationEngine.ValidPoint coordinate;

        public Country Country { get; set; }
        
        public Hacker(int id, int skillLevel)
        {
            _id = id;
            _skillLevel = skillLevel;
            _cash = 0;
            _isAlive = true;
        }


        public void Action()
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            return _isAlive;
        }
    

        public void UpdateCashAmount(int amount)
        {
            _cash += amount;
        }

        public void IncreaseSkillLevel()
        {
            if(_skillLevel <= 9)
            {
                _skillLevel++;
            }
        }

        public void SetDead()
        {
            _isAlive = false;
        }

        public Country C
        {
            get { return Country; }
            set { Country = value; }

        }

        public SimulationEngine.ValidPoint Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }
    }
}
