using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring
{
    using Datastructures;
    using DoubleMap = Dictionary<ulong, double>;

    class SuzukiScoringFunction : ScoringFunction
    {
        public SuzukiScoringFunction(BayesianNetwork network, ADTree adTree)
        {
            this.adTree = adTree;
            this.network = network;
        }

        public override double CalculateScore(int variable, Varset parents, DoubleMap cache)
        {
            Varset parentsCp = new Varset(parents);
            Varset variables = new Varset(parentsCp);
            variables.Set(variable, true);

            ContingencyTableNode ctPa = adTree.MakeContab(parentsCp);
            ContingencyTableNode ctPaX = adTree.MakeContab(variables);

            double qPa = CalculateLocalScore(parentsCp, ctPa);
            double qPaX = CalculateLocalScore(variables, ctPa);

            return Math.Log(qPaX / qPa);
        }

        private double CalculateLocalScore(Varset variables, ContingencyTableNode ct)
        {
            // LocalScoreを計算する
            int r = 1;
            for (int i = 0; i < network.Size(); i++)
            {
                if (variables.Get(i))
                {
                    r *= network.GetCardinality(i);
                }
            }
            int[] formatedCt = Enumerable.Repeat<int>(0, r).ToArray();
            int index = 0;
            FormatContingencyTable(ct, variables, -1, formatedCt, ref index);

            double a = 0.5;
            double n = adTree.RecordCount;
            double gammaLnRA = ScoreCalculator.GammaLn(r * a);
            double rGammaLnA = r * ScoreCalculator.GammaLn(a);
            double gammaLnNRA = ScoreCalculator.GammaLn(n + r * a);
            double gammaLnXCA = 0;
            for (int i = 0; i < r; i++)
            {
                gammaLnXCA += ScoreCalculator.GammaLn(formatedCt[i] + a);
            }

            double score = (gammaLnRA + gammaLnXCA) - (rGammaLnA + gammaLnNRA);
            return score;
        }

        private void FormatContingencyTable(ContingencyTableNode ct, Varset variables, int previousVariable, int [] formatedCt, ref int index)
        {
            if (ct.IsLeaf())
            {
                formatedCt[index] = ct.Value;
                index++;
                return;
            }

            int thisVariable = previousVariable + 1;
            for (; thisVariable < network.Size(); thisVariable++)
            {
                if (variables.Get(thisVariable))
                {
                    break;
                }
            }
            if (thisVariable == network.Size())
            {
                return;
            }

            for (int i = 0; i < network.GetCardinality(thisVariable); i++)
            {
                ContingencyTableNode child = ct.GetChild(i);
                FormatContingencyTable(ct, variables, thisVariable, formatedCt, ref index);
            }
        }

        private ADTree adTree;
        private BayesianNetwork network;
    }
}
