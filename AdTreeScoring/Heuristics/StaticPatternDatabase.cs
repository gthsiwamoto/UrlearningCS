using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;
using Scoring;
using BestScoreCalculators;

namespace Heuristics
{
    using DoubleMap = Dictionary<ulong, double>;

    class StaticPatternDatabase : Heuristic
    {
        public StaticPatternDatabase() { }
        public StaticPatternDatabase(int variableCount, int patternDatabaseCount, bool isRandom)
        {
            this.variableCount = variableCount;
            this.patternDatabaseCount = patternDatabaseCount;
            this.isRandom = isRandom;
            ancestors = new Varset(variableCount);
            scc = new Varset(variableCount);
            scc.SetAll(true);
        }

        public StaticPatternDatabase(int variableCount, int patternDatabaseCount, bool isRandom, Varset ancestors, Varset scc)
        {
            this.variableCount = variableCount;
            this.patternDatabaseCount = patternDatabaseCount;
            this.isRandom = isRandom;
            this.ancestors = ancestors;
            this.scc = scc;
        }

        public override void Initialize(List<BestScoreCalculator> spgs)
        {
            // create the variable set bitmasks
            int x = 0;

            Varset allVariables = new Varset(variableCount);
            allVariables = allVariables.Or(scc);

            Varset remainingVariables = new Varset(variableCount);
            remainingVariables = remainingVariables.Or(scc);

            int remainingCount = scc.Cardinality();
            int var = scc.FindFirst();

            int patternDatabaseSize = (int)Math.Ceiling((double)remainingCount / patternDatabaseCount);

            for (int pdI = 0; pdI < patternDatabaseCount; pdI++)
            {
                Varset empty = new Varset(variableCount);
                variableSets.Add(empty);

                int variableSetSize = 0;

                if (isRandom)
                {
                    // create the variable set randomly
                    for (; variableSetSize < patternDatabaseSize && remainingCount > 0; variableSetSize++, remainingCount--)
                    {
                        variableSets[pdI].Set(var, true);
                        var = scc.FindFirst();
                        x++;
                    }
                }
                else
                {
                    for (variableSetSize = 0; variableSetSize < patternDatabaseSize && x < remainingCount; variableSetSize++)
                    {
                        variableSets[pdI].Set(var, true);
                        var = scc.FindNext(var);
                        x++;
                    }
                }

                Console.Write("Creating pattern database: ");
                variableSets[pdI].Print();

                DoubleMap patternDatabase = new DoubleMap((int)Math.Pow(2, variableSetSize));
                patternDatabases.Add(patternDatabase);

                CreatePatternDatabase(allVariables, variableSets[pdI], variableSetSize, spgs, patternDatabases[pdI]);
            }
        }

        public void Initialize(List<BestScoreCalculator> spgs, List<Varset> variableSets, Varset ancestors, Varset scc)
        {
            variableCount = spgs.Count;
            this.variableSets = variableSets;
            isRandom = false;
            patternDatabaseCount = variableSets.Count;
            this.ancestors = ancestors;
            this.scc = scc;

            Varset allVariables = new Varset(variableCount);
            allVariables = allVariables.Or(scc);

            for (int pdI = 0; pdI < patternDatabaseCount; pdI++)
            {
                int variableSetSize = 0;
                variableSetSize = variableSets[pdI].Cardinality();

                Console.Write("Creating pattern database: ");
                variableSets[pdI].Print();

                DoubleMap patternDatabase = new DoubleMap((int)Math.Pow(2, variableSetSize));
                patternDatabases.Add(patternDatabase);

                CreatePatternDatabase(allVariables, variableSets[pdI], variableSetSize, spgs, patternDatabases[pdI]);
            }
        }

        public override int Size()
        {
            int size = 0;
            for (int pdI = 0; pdI < patternDatabaseCount; pdI++)
            {
                size += patternDatabases[pdI].Count;
            }
            return size;
        }

        public override double h(Varset variables, bool complete)
        {
            double h = 0;

            Varset mask = new Varset(variableCount);
            mask.SetAll(true);

            Varset remaining = new Varset(variables.Not());

            // use the mask to make sure "remaining" only has valid variables left
            remaining = remaining.And(mask);

            for (int pdI = 0; pdI < patternDatabaseCount; pdI++)
            {
                Varset vs = new Varset(variableSets[pdI].And(remaining));

                if (vs.Equals(remaining))
                {
                    complete = true;
                    return patternDatabases[pdI][vs.ToULong()];
                }
                ulong hoge = vs.ToULong();
                h += patternDatabases[pdI][vs.ToULong()];
            }
            Console.WriteLine("static heuristic: " + h);

            return h;
        }

        private int GetRandomVariable(Varset remainingVariables, int remainingCount, int variableCount)
        {
            Random rand = new Random();
            int r = rand.Next() % remainingCount;

            int counter = 0;
            for (int x = 0; x < variableCount; x++)
            {
                if (remainingVariables.Get(x))
                {
                    if (counter == 0)
                    {
                        return x;
                    }
                    counter++;
                }
            }
            return -1;
        }

        private void CreatePatternDatabase(Varset allVariables, Varset variableSet, int variableSetSize, List<BestScoreCalculator> spgs, DoubleMap patternDatabase)
        {
            DoubleMap previousLayer = new DoubleMap();

            previousLayer[allVariables.ToULong()] = 0;

            // create the pattern database by performing a reverse bfs
            for (int layer = 0; layer <= variableSetSize; layer++)
            {
                DoubleMap currentLayer = new DoubleMap();
                foreach (KeyValuePair<ulong, double> kvp in previousLayer)
                {
                    Varset key = new Varset(kvp.Key);
                    double value = kvp.Value;

                    // expand this subnetwork
                    Expand(key, value, variableSet, spgs, currentLayer);

                    // add (the inverse of) these variables to the pd
                    Varset pattern = new Varset(variableSet.And(key.Not()));

                    // also remove the ancestors
                    pattern = pattern.And(ancestors.Not());
                    patternDatabase[pattern.ToULong()] = value;
                }
                previousLayer = currentLayer;
            }

            // add the last layer to the pd (this should only be one node...)
            foreach (KeyValuePair<ulong, double> kvp in previousLayer)
            {
                Varset key = new Varset(kvp.Key);
                double value = kvp.Value;

                patternDatabase[variableSet.And(key.Not()).ToULong()] = value;
            }
        }

        private void Expand(Varset subNetwork, double g, Varset variableSet, List<BestScoreCalculator> spgs, DoubleMap currentLayer)
        {
            // for each remaining leaf
            for (int leaf = 0; leaf < variableCount; leaf++)
            {
                // is it still remaining and one of my variables?
                if (!subNetwork.Get(leaf) || !variableSet.Get(leaf))
                {
                    continue;
                }

                // find the best parents
                Varset parentChoices = new Varset(variableCount);
                parentChoices = subNetwork.Or(ancestors);

                double newG = spgs[leaf].GetScore(parentChoices) + g;

                // duplicate detection
                Varset newSubNetwork = Varset.ClearCopy(subNetwork, leaf);
                double oldG = currentLayer.ContainsKey(newSubNetwork.ToULong()) ? currentLayer[newSubNetwork.ToULong()] : 0;
                //double oldG = currentLayer[newSubNetwork.ToULong()];

                if (oldG == 0 || newG < oldG)
                {
                    currentLayer[newSubNetwork.ToULong()] = newG;
                }
            }
        }

        private int variableCount;
        private bool isRandom;
        private int patternDatabaseCount;
        public int PatternDatabaseCount
        {
            get
            {
                return patternDatabaseCount;
            }

            set
            {
                patternDatabaseCount = value;
            }
        }
        private Varset ancestors;
        private Varset scc;
        private List<Varset> variableSets = new List<Varset>();
        private List<DoubleMap> patternDatabases = new List<DoubleMap>();
    }
}
