using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;
using Scoring;

namespace BestScoreCalculators
{
    class SparseParentList : BestScoreCalculator
    {
        public SparseParentList(int variable, int variableCount)
        {
            this.variable = variable;
            this.variableCount = variableCount;
        }

        public override void Initialize(ScoreCache scoreCache)
        {
            // populate the score and parents arrays
            List<KeyValuePair<ulong, double>> spg = scoreCache.GetCache(variable).ToList();

            spg.Sort(Comparison);

            for (int i = 0; i < spg.Count; i++)
            {
                parents.Add(new Varset(spg[i].Key));
                scores.Add(spg[i].Value);
            }
        }

        public override Varset GetParents()
        {
            return parents[bestIndex];
        }

        public double GetBestScore()
        {
            return scores[0];
        }

        public override double GetScore(Varset pars)
        {
            for (bestIndex = 0; bestIndex < scores.Count; bestIndex++)
            {
                if (pars.And(parents[bestIndex]).Equals(parents[bestIndex]))
                {
                    break;
                }

                if (bestIndex == scores.Count)
                {
                    return Double.MaxValue;
                }
            }
            return scores[bestIndex];
        }

        public override int Size()
        {
            return parents.Count;
        }

        public override void Print()
        {
            Console.WriteLine("Sparse Parent Bitwise, variable: " + variable + ", size: " + Size());
        }

        private static int Comparison(KeyValuePair<ulong, double> x, KeyValuePair<ulong, double> y)
        {
            if (x.Value < y.Value)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        private int variableCount;
        private int variable;
        private int bestIndex;
        private List<Varset> parents = new List<Varset>();
        private List<double> scores = new List<double>();
    }
}
