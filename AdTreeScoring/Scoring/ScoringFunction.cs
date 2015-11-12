using Datastructures;
using System.Collections.Generic;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;

    class ScoringFunction
    {
        public virtual double CalculateScore(int variable, Varset parents, DoubleMap cache) { return 0; }
    }
}
