using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring
{
    class ScoreCalculator
    {
        public ScoreCalculator(ScoringFunction scoringFunction, int maxParents, int variableCount, int runningTime, Constraints constraints)
        {
            this.scoringFunction = scoringFunction;
            this.maxParents = maxParents;
            this.variableCount = variableCount;
            this.runningTime = runningTime;
            this.constraints = constraints;
        }

        private ScoringFunction scoringFunction;
        private int maxParents;
        private int variableCount;
        private int runningTime;
        private Constraints constraints;
    }
}
