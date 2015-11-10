using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;

namespace Scoring
{
    class BICScoringFunction : ScoringFunction
    {
        public BICScoringFunction(BayesianNetwork network, RecordFile recordFile, LogLikelihoodCalculator llc, Constraints constraints)
        {
            this.network = network;
            this.llc = llc;
            this.constraints = constraints;
            baseComplexityPenalty = Math.Log(recordFile.Size()) / 2;
        }

        private BayesianNetwork network;
        private Constraints constraints;
        private LogLikelihoodCalculator llc;
        private double baseComplexityPenalty;
    }
}
