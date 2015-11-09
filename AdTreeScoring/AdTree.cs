using System;
using System.Collections;
using Datastructures;
using System.Collections.Generic;

namespace AdTreeScoring
{
    class AdTree
    {
        public AdTree() { }
        public AdTree(int rMin)
        {
            this.rMin = rMin;
        }

        public AdTree(int rMin, BayesianNetwork network, RecordFile recordFile)
        {
            this.rMin = rMin;
            Initialize(network, recordFile);
            CreateTree();
        }

        private void Initialize(BayesianNetwork network, RecordFile recordFile)
        {
            this.network = network;
            recordCount = recordFile.Size();
            zero = new Varset(network.Size());

            consistentRecords = network.GetConsistentRecords(recordFile);
        }

        private void CreateTree()
        {
            BitArray countIndices = new BitArray(recordCount);
            countIndices.SetAll(true);
            Varset empty = new Varset(network.Size());
            root = MakeAdTree(0, countIndices, 0, empty);
        }

        private AdNode MakeAdTree(int i, BitArray recordNums, int depth, Varset variables)
        {
            // since this is index i, there are (variableCount - i) remaining variables.
            // therefore, it will have that many children
            AdNode adn = new AdNode(network.Size() - i, recordNums.Count);

            // check if we should just use a leaf list
            if (adn.Count < rMin)
            {
                BitArray leafList = new BitArray(recordNums);
                adn.LeafList = leafList;
                return adn;
            }

            // for each of the remaining variables
            for (int j = i; j < network.Size(); j++)
            {
                Console.WriteLine("DEBUG j");
                Console.WriteLine(j);

                // create a vary node
                Varset newVariables = variables.Set(j);
                VaryNode child = MakeVaryNode(j, recordNums, depth, newVariables);
                adn.SetChild(j - i, child);
            }

            return adn;
        }

        private VaryNode MakeVaryNode(int i, BitArray recordNums, int depth, Varset variables)
        {
            // this node will have variableCardinalities[i] children
            VaryNode vn = new VaryNode(network.GetCardinality(i));

            // split into childNums
            List<BitArray> childNums = new List<BitArray>();

            int mcv = -1;
            int mcvCount = -1;
            for (int k = 0; k < network.GetCardinality(i); k++)
            {
                childNums.Add(new BitArray(recordCount));
                childNums[k].Or(recordNums);
                childNums[k].And(consistentRecords[i][k]);

                // also look for the mcv
                int count = childNums[k].Count;
                if (count > mcvCount)
                {
                    mcv = k;
                    mcvCount = count;
                }
            }

            // update the mcv
            vn.Mcv = mcv;

            // otherwise, rescue
            for (int k = 0; k < network.GetCardinality(i); k++)
            {
                if (k == mcv || childNums[k].Count == 0)
                {
                    continue;
                }

                AdNode child = MakeAdTree(i + 1, childNums[k], depth + 1, variables);
                vn.SetChild(k, child);
            }

            return vn;

        }

        private int rMin;
        private BayesianNetwork network;
        private int recordCount;
        private Varset zero;
        private AdNode root;
        private List<List<BitArray>> consistentRecords;
    }
}
