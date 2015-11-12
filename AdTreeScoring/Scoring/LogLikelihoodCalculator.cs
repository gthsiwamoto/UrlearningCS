using System;
using System.Collections.Generic;
using Datastructures;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring
{
    class LogLikelihoodCalculator
    {
        public LogLikelihoodCalculator(ADTree adTree, BayesianNetwork network, List<double> ilogi)
        {
            Initialize(adTree, network, ilogi);
        }

        public static List<double> GetLogCache(int recordCount)
        {
            List<double> logCache = new List<double>();
            logCache.Add(0.0);
            for (int i = 1; i < recordCount + 2; i++)
            {
                double l = i * Math.Log(i);
                logCache.Add(l);
            }
            return logCache;
        }

        public double Calculate(int variable, Varset parents)
        {
            Dictionary<ulong, int> paCounts = new Dictionary<ulong, int>();
            return Calculate(variable, parents, paCounts);
        }

        public double Calculate(int variable, Varset parents, Dictionary<ulong, int> paCounts)
        {
            parents.Set(variable, true);
            double score = 0;

            ContingencyTableNode ct = adTree.MakeContab(parents);

            Calculate(ct, 1, 0, paCounts, variable, parents, -1, ref score);

            foreach(KeyValuePair<ulong, int> pair in paCounts)
            {
                score -= ilogi[pair.Value];
            }

            parents.Set(variable, false);

            return score;
        }

        private void Calculate(ContingencyTableNode ct, ulong currentBase, ulong index, Dictionary<ulong, int> paCounts, int variable, Varset variables, int previousVariable, ref double score)
        {
            // if this is a leaf
            if (ct.IsLeaf())
            {
                // update the instantiation count of this set of parents
                int count = paCounts.ContainsKey(index) ? paCounts[index] : 0;
                count += ct.Value;
                paCounts[index] = count;

                // update the score for this variable, parent instantiation
                score += ilogi[ct.Value];
                return;
            }

            // which actual variable are we looking at
            int thisVariable = previousVariable + 1;
            for (; thisVariable < network.Size(); thisVariable++)
            {
                if (variables.Get(thisVariable))
                {
                    break;
                }
            }

            // update the base and index if this is part of the parent set
            ulong nextBase = currentBase;
            if (thisVariable != variable)
            {
                nextBase *= (ulong)network.GetCardinality(thisVariable);
            }

            // recurse
            for (int k = 0; k < network.GetCardinality(thisVariable); k++)
            {
                ContingencyTableNode child = ct.GetChild(k);
                if (child != null)
                {
                    ulong newIndex = index;
                    if (thisVariable != variable)
                    {
                        newIndex += currentBase * (ulong)k;
                    }
                    Calculate(child, nextBase, newIndex, paCounts, variable, variables, thisVariable, ref score);
                }
            }
            
        }

        private void Initialize(ADTree adTree, BayesianNetwork network, List<double> ilogi)
        {
            this.adTree = adTree;
            this.network = network;
            this.ilogi = ilogi;
        }

        private ADTree adTree;
        private BayesianNetwork network;
        private List<double> ilogi = new List<double>();
    }
}
