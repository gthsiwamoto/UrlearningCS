using System;
using System.Collections.Generic;
using System.IO;
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

            ulong paIndex = parentsCp.ToULong();
            double paTmp = 0;
            if (!localScoreMap.ContainsKey(paIndex))
            {
                paTmp = CalculateLocalScore(parentsCp);
                localScoreMap[paIndex] = paTmp;
            }
            double lnQPa = localScoreMap[paIndex];

            ulong vaIndex = variables.ToULong();
            double vaTmp = 0;
            if (!localScoreMap.ContainsKey(vaIndex))
            {
                vaTmp = CalculateLocalScore(variables);
                localScoreMap[vaIndex] = vaTmp;
            }
            double lnQPaX = localScoreMap[vaIndex];

            double score = lnQPaX - lnQPa;
            Console.WriteLine("variable: " + variable);
            Console.Write("parents: ");
            parents.Print();
            Console.Write("variables: ");
            variables.Print();
            Console.WriteLine("score: " + score + "\n");
            return score;
        }

        public double SuzukiDelta(Varset variables)
        {
            Varset variablesCp = new Varset(variables);
            Varset allVariables = new Varset(variables);

            for (int i = 0; i < network.Size(); i++)
            {
                allVariables.Set(i, true);
            }
            if (!localScoreMap.ContainsKey(allVariables.ToULong()))
            {
                localScoreMap[allVariables.ToULong()] = CalculateLocalScore(allVariables);
            }
            double lnQV = localScoreMap[allVariables.ToULong()];

            if (!localScoreMap.ContainsKey(variablesCp.ToULong()))
            {
                localScoreMap[variablesCp.ToULong()] = CalculateLocalScore(variablesCp);
            }
            double lnQS = localScoreMap[variablesCp.ToULong()];

            double tmp = 0;
            for (int i = 0; i < network.Size(); i++)
            {
                if (!variablesCp.Get(i))
                {
                    Varset variablesTmp = new Varset(allVariables);
                    variablesTmp.Set(i, false);

                    if (!localScoreMap.ContainsKey(variablesTmp.ToULong()))
                    {
                        localScoreMap[variablesTmp.ToULong()] = CalculateLocalScore(variablesTmp);
                    }

                    tmp += localScoreMap[variablesTmp.ToULong()] - lnQV;
                }
            }

            double delta = lnQS - lnQV - tmp;
            return delta > 0 ? delta : 0;
        }

        private double CalculateLocalScore(Varset variables)
        {
            Varset variablesCp = new Varset(variables);
            ContingencyTableNode ct = adTree.MakeContab(variablesCp);
            int r = 1;
            for (int i = 0; i < network.Size(); i++)
            {
                if (variablesCp.Get(i))
                {
                    r *= network.GetCardinality(i);
                }
            }
            List<int> formatedCt = new List<int>();
            FormatContingencyTable(ct, variablesCp, -1, formatedCt);

            double a = 0.5;
            double n = adTree.RecordCount;
            double gammaLnRA = ScoreCalculator.GammaLn(r * a);
            double rGammaLnA = r * ScoreCalculator.GammaLn(a);
            double gammaLnNRA = ScoreCalculator.GammaLn(n + r * a);
            double gammaLnXCA = 0;
            for (int i = 0; i < r; i++)
            {
                int c = 0;
                if (i < formatedCt.Count)
                {
                    c = formatedCt[i];
                }
                gammaLnXCA += ScoreCalculator.GammaLn(c + a);
            }

            double localScore = (gammaLnRA + gammaLnXCA) - (rGammaLnA + gammaLnNRA);
            return localScore;
        }

        private void FormatContingencyTable(ContingencyTableNode ct, Varset variables, int previousVariable, List<int> formatedCt)
        {
            if (ct.IsLeaf())
            {
                if (variables.ToULong() != 0)
                {
                    formatedCt.Add(ct.Value);
                }
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
                if (child == null)
                {
                    return;
                }
                FormatContingencyTable(child, variables, thisVariable, formatedCt);
            }
        }

        public void OutputLocalScoreMap(string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);
            foreach (KeyValuePair<ulong, double> kvp in localScoreMap)
            {
                sw.Write(kvp.Key + "," + kvp.Value + "\n");
            }
            sw.Close();
        }

        public void ReadLocalScoreMap(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                string read_line = sr.ReadLine();
                string [] map = read_line.Split(',');
                localScoreMap[ulong.Parse(map[0])] = double.Parse(map[1]);
            }
            sr.Close();
        }

        private ADTree adTree;
        private BayesianNetwork network;
        private Dictionary<ulong, double> localScoreMap = new Dictionary<ulong, double>();
    }
}
