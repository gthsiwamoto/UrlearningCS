using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;
using MathNet.Numerics;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;

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

        public void CalculateScores(int variable, DoubleMap cache)
        {
            outOfTime = false;

            if (runningTime > 0)
            {
                // TODO 時間制限がある場合
            }
            else
            {
                CalculateScoresInternal(variable, cache);
            }
        }

        private void CalculateScoresInternal(int variable, DoubleMap cache)
        {
            // calculate initial score
            Varset empty = new Varset(variableCount + 1); // 注意: c++だと0
            double score = scoringFunction.CalculateScore(variable, empty, cache);

            if (score < 1)
            {
                cache[empty.ToULong()] = score;
            }

            int prunedCount = 0;

            for (int layer = 1; layer <= maxParents && !outOfTime; layer++)
            {
                // DEBUG
                Console.WriteLine("layer: " + layer + ", prunedCount: " + prunedCount);

                Varset variables = new Varset(variableCount + 1); // 注意: c++だと0
                for (int i = 0; i < layer; i++)
                {
                    variables.Set(i, true);
                }

                Varset max = new Varset(variableCount);
                max.Set(variableCount, true);

                while (variables.LessThan(max) && !outOfTime)
                {
                    if (!variables.Get(variable))
                    {
                        score = scoringFunction.CalculateScore(variable, variables, cache);

                        if (score < 0)
                        {
                            cache[variables.ToULong()] = score;
                        }
                        else
                        {
                            prunedCount++;
                        }
                    }

                    variables = variables.NextPermutation();
                }

                if (!outOfTime)
                {
                    highestCompletedLayer = layer;
                }
            }
        }

        public void Prune(DoubleMap cache)
        {
            List<KeyValuePair<ulong, double>> pairs = new List<KeyValuePair<ulong, double>>();
            foreach (KeyValuePair<ulong, double> kvp in cache)
            {
                pairs.Add(kvp);
            }
            pairs.Sort(Comparison);

            // keep track of the ones that have been pruned
            BitArray prunedSet = new BitArray(pairs.Count);
            for (int i = 0; i < pairs.Count; i++)
            {
                if (prunedSet.Get(i))
                {
                    continue;
                }

                Varset pi = new Varset(pairs[i].Key);

                // make sure this variable set is not in an incomplete last layer
                if (pi.Cardinality() > highestCompletedLayer)
                {
                    prunedSet.Set(i, true);
                    continue;
                }

                for (int j = i + 1; j < pairs.Count; j++)
                {
                    if (prunedSet.Get(j))
                    {
                        continue;
                    }

                    // check if parents[i] is a subset of parents[j]
                    Varset pj = new Varset(pairs[j].Key);

                    if (pi.And(pj).Equals(pi)) // 部分集合かどうかの判定
                    {
                        // then we can prune pj
                        prunedSet.Set(j, true);
                        cache.Remove(pj.ToULong());
                    }
                }
            }
        }

        private static int Comparison(KeyValuePair<ulong, double> x, KeyValuePair<ulong, double> y)
        {
            double val = x.Value - y.Value;
            if (Math.Abs(val) > 2 * Double.Epsilon)
            {
                if (val > 0)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (x.Key < y.Key)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private ScoringFunction scoringFunction;
        private int maxParents;
        private int variableCount;
        private int runningTime;
        private Constraints constraints;
        private bool outOfTime;
        private int highestCompletedLayer;
        
        public static double GammaLn(double z)
        {
            return z == 1 ? 0 : SpecialFunctions.GammaLn(z);
        }
    }
}
