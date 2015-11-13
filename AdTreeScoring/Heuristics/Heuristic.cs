using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestScoreCalculators;
using Datastructures;

namespace Heuristics
{
    class Heuristic
    {
        public virtual double h(Varset subNetwork, bool complete)
        {
            return 0;
        }

        public virtual void Initialize(List<BestScoreCalculator> spgs) { }

        public virtual int Size()
        {
            return 0;
        }

        public virtual void Print() { }
        public virtual void PrintStatistics() { }
    }
}
