using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;

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
                cache[empty.ToLong()] = score;
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

                        // DEBUG
                        Console.WriteLine(variable +  ", " + score);

                        if (score < 0)
                        {
                            cache[variables.ToLong()] = score;
                        }
                        else
                        {
                            prunedCount++;
                        }
                    }

                    //Console.Write("before: ");
                    //variables.Print();
                    variables = variables.NextPermutation();
                    //Console.Write("after:  ");
                    //variables.Print();
                    //Console.Write("max:    ");
                    //max.Print();
                    //Console.WriteLine(variables.LessThan(max));
                }
            }
        }

        private ScoringFunction scoringFunction;
        private int maxParents;
        private int variableCount;
        private int runningTime;
        private Constraints constraints;
        private bool outOfTime;
    }
}
