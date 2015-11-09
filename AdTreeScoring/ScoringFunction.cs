using Datastructures;
using System.Collections.Generic;

namespace Scoring
{
    class ScoringFunction
    {
        public virtual double CalculateScore(int variable, Varset parents, Dictionary<Varset, double> cache) { return 0; }
    }
}
