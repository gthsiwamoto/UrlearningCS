using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;

    class BICScoringFunction : ScoringFunction
    {
        public BICScoringFunction(BayesianNetwork network, RecordFile recordFile, LogLikelihoodCalculator llc, Constraints constraints)
        {
            this.network = network;
            this.llc = llc;
            this.constraints = constraints;
            baseComplexityPenalty = Math.Log(recordFile.Size()) / 2;
        }

        public override double CalculateScore(int variable, Varset parents, DoubleMap cache)
        {
            Varset parentsCp = new Varset(parents);
            // TODO check if this violates the constraints
            //if(constraints != NULL && !constraints.SatisfiesConstraints(variable, parents))
            //{
            //}

            // check for prunning
            double tVal = T(variable, parentsCp);

            for (int x = 0; x < network.Size(); x++)
            {
                if (parentsCp.Get(x))
                {
                    parentsCp.Set(x, false);

                    // TODO check the constraints
                    //if (invalidParents.Count > 0 && invalidParents.Contains(parentsCp.ToULong()))
                    //{
                    //    parentsCp.Set(x);
                    //    continue;
                    //}

                    double tmp = cache.ContainsKey(parentsCp.ToULong()) ? cache[parentsCp.ToULong()] : 0;
                    if (tmp + tVal > 0)
                    {
                        return 0;
                    }

                    parentsCp.Set(x, true);
                }
            }

            double score = llc.Calculate(variable, parentsCp);

            // structure penalty
            score -= tVal * baseComplexityPenalty;
            //Console.WriteLine("score: " + score);

            return score;
        }

        private double T(int variable, Varset parents)
        {
            double penalty = network.GetCardinality(variable) - 1;
            for (int pa = 0; pa < network.Size(); pa++)
            {
                if (parents.Get(pa))
                {
                    penalty *= network.GetCardinality(pa);
                }
            }
            return penalty;
        }

        private BayesianNetwork network;
        private Constraints constraints;
        private LogLikelihoodCalculator llc;
        private double baseComplexityPenalty;
        private HashSet<ulong> invalidParents;
    }
}
