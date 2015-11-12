using System;
using System.Collections.Generic;
using Datastructures;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;

    struct Scratch
    {
        /*+
         * The score calculation is for this variable.
         */ 
        public int Variable;

        /**
         * Scratch variables for calculation the most recent score across different
         * function calls.
         */
        public double Score;
        public double Lri;
        public double Lgij;
        public double Lgijk;
        public double Aij;
        public double Aijk;

        /**
         * A cache to store all of the parent sets which were pruned because they
         * violated the constraints. We need to store these because they cannot be
         * used when checking for weak pruning.
         */
        public HashSet<ulong> invalidParents;
    }

    class BDeuScoringFunction : ScoringFunction
    {
        public BDeuScoringFunction(double ess, BayesianNetwork network, ADTree adTree, Constraints constraints)
        {
            this.network = network;
            this.adTree = adTree;
            this.constraints = constraints;
            this.ess = ess;

            for (int x = 0; x < network.Size(); x++)
            {
                Scratch s = new Scratch();
                s.Variable = x;
                s.Lri = Math.Log(network.GetCardinality(x));
                s.invalidParents = new HashSet<ulong>();
                scratchSpace.Add(s);
            }
        }

        public override double CalculateScore(int variable, Varset parents, DoubleMap cache)
        {
            Scratch s = scratchSpace[variable];

            // TODO check if this violates the constraints
            //if (constraints != NULL && !constraints->satisfiesConstraints(variable, parents))
            //{
            //    s->invalidParents.insert(parents);
            //    return 1;
            //}

            for (int x = 0; x < network.Size(); x++)
            {
                if (parents.Get(x))
                {
                    parents.Set(x, false);

                    // TODO check the constraints
                    //if (invalidParents.Count > 0 && invalidParents.Contains(parents.ToULong()))
                    //{
                    //    // we cannot say anything if we skipped this because of constraints
                    //    parents.Set(x, true);
                    //    continue;
                    //}

                    parents.Set(x, true);
                }
            }

            Lg(parents, ref s);
            Varset variables = new Varset(parents);
            variables.Set(variable, true);

            s.Score = 0;

            ContingencyTableNode ct = adTree.MakeContab(variables);

            Dictionary<ulong, int> paCounts = new Dictionary<ulong, int>();
            Calculate(ct, 1, 0, paCounts, variables, -1, ref s);

            // check constraints (Theorem 9 from de Campos and Ji '11)
            // only bother if the alpha bound is okay
            // check if can prune
            if (s.Aij <= 0.8349)
            {
                double bound = -1.0 * ct.LeafCount * s.Lri;

                // check each subset
                for (int x = 0; x < network.Size(); x++)
                {
                    if (parents.Get(x))
                    {
                        parents.Set(x, false);

                        // check the constraints
                        //if (s->invalidParents.find(parents) != s->invalidParents.end())
                        //{
                        //    // we cannot say anything if we skipped this because of constraints
                        //    VARSET_SET(parents, x);
                        //    continue;
                        //}

                        double tmp = cache.ContainsKey(parents.ToULong()) ? cache[parents.ToULong()] : 0;

                        // if the score is larger (better) than the bound, then we can prune
                        if (tmp > bound)
                        {
                            return 0;
                        }

                        parents.Set(x, true);
                    }
                }
            }

            foreach (KeyValuePair<ulong, int> kvp in paCounts)
            {
                s.Score += s.Lgij;
                s.Score -= ScoreCalculator.GammaLn(s.Aij + kvp.Value);
            }

            //parents.Print();
            //Console.WriteLine(variable);
            return s.Score;
        }

        private void Lg(Varset parents, ref Scratch s)
        {
            int r = 1;
            for (int pa = 0; pa < network.Size(); pa++)
            {
                if (parents.Get(pa))
                {
                    r *= network.GetCardinality(pa);
                }
            }

            s.Aij = ess / r;
            s.Lgij = ScoreCalculator.GammaLn(s.Aij);
            r *= network.GetCardinality(s.Variable);
            s.Aijk = ess / r;
            s.Lgijk = ScoreCalculator.GammaLn(s.Aijk);
        }

        private void Calculate(ContingencyTableNode ct, ulong currentBase, ulong index, Dictionary<ulong, int> paCounts, Varset variables, int previousVariable, ref Scratch s)
        {
            Varset variablesCp = new Varset(variables);
            // if this is a leaf in the AD-tree
            if (ct.IsLeaf())
            {
                // update the instantiation count of this set of parents
                int count = paCounts.ContainsKey(index) ? paCounts[index] : 0;
                count += ct.Value;

                if (count > 0)
                {
                    paCounts[index] = count;

                    // update the score for this variable, parent instantiation
                    double temp = ScoreCalculator.GammaLn(s.Aijk + ct.Value);
                    s.Score -= s.Lgijk;
                    s.Score += temp;
                }
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
            if (thisVariable != s.Variable)
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
                    if (thisVariable != s.Variable)
                    {
                        newIndex += currentBase * (ulong)k;
                    }
                    Calculate(child, nextBase, newIndex, paCounts, variables, thisVariable, ref s);
                }
            }
        }

        private ADTree adTree;
        private BayesianNetwork network;
        private Constraints constraints;
        private double ess;
        private HashSet<ulong> invalidParents = new HashSet<ulong>();
        private List<Scratch> scratchSpace = new List<Scratch>();
    }
}
