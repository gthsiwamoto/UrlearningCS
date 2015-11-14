using System;
using System.Collections;
using System.Collections.Generic;
using Datastructures;

namespace Scoring
{
    class ADTree
    {
        public ADTree() { }
        public ADTree(int rMin)
        {
            this.rMin = rMin;
        }

        public ADTree(int rMin, BayesianNetwork network, RecordFile recordFile)
        {
            this.rMin = rMin;
            Initialize(network, recordFile);
            CreateTree();
        }

        public ContingencyTableNode MakeContab(Varset variables)
        {
            return MakeContab(variables, root, -1);
        }

        private ContingencyTableNode MakeContab(Varset remainingVariables, ADNode node, int nodeIndex)
        {
            // check base case
            if (remainingVariables.Equals(zero))
            {
                ContingencyTableNode ctn = new ContingencyTableNode(node.Count, 0, 1);
                return ctn;
            }

            int firstIndex = remainingVariables.FindFirst();
            int n = network.GetCardinality(firstIndex);
            VaryNode vn = node.GetChild(firstIndex - nodeIndex - 1);
            ContingencyTableNode ct = new ContingencyTableNode(0, n, 0);
            Varset newVariables = Varset.ClearCopy(remainingVariables, firstIndex);

            ContingencyTableNode ctMcv = MakeContab(newVariables, node, nodeIndex);

            for (int k = 0; k < n; k++)
            {
                if (vn.GetChild(k) == null)
                {
                    continue;
                }

                ADNode adn = vn.GetChild(k);

                ContingencyTableNode child = null;
                if (adn.LeafList.Count == 0) // これ注意
                {
                    child = MakeContab(newVariables, adn, firstIndex);
                }
                else
                {
                    child = MakeContabLeafList(newVariables, adn.LeafList);
                }

                ct.SetChild(k, child);
                ct.LeafCount += child.LeafCount;

                ctMcv.Subtract(ct.GetChild(k));
            }
            ct.SetChild(vn.Mcv, ctMcv);
            ct.LeafCount += ctMcv.LeafCount;

            return ct;
        }

        private ContingencyTableNode MakeContabLeafList(Varset variables, BitArray records)
        {
            Varset variablesCp = new Varset(variables);
            if (variablesCp.Equals(zero))
            {
                int count = 0;
                for (int i = 0; i < records.Count; i++)
                {
                    if (records[i])
                    {
                        count += 1;
                    }
                }
                return new ContingencyTableNode(count, 0, 1);
            }

            int firstIndex = variables.FindFirst();
            int cardinality = network.GetCardinality(firstIndex);
            ContingencyTableNode ct = new ContingencyTableNode(0, cardinality, 0);
            variablesCp.Set(firstIndex, false);
            Varset remainingVariables = new Varset(variablesCp);
            for (int k = 0; k < cardinality; k++)
            {
                BitArray r = new BitArray(recordCount);
                r = r.Or(records);
                r = r.And(consistentRecords[firstIndex][k]);

                int count = 0;
                for (int i = 0; i < r.Count; i++)
                {
                    if (r[i])
                    {
                        count += 1;
                    }
                }
                if (count > 0)
                {
                    ContingencyTableNode child = MakeContabLeafList(remainingVariables, r);
                    ct.SetChild(k, child);
                    ct.LeafCount += child.LeafCount;
                }
            }
            return ct;
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
            root = MakeADTree(0, countIndices, 0, empty);
        }

        private ADNode MakeADTree(int i, BitArray recordNums, int depth, Varset variables)
        {
            // since this is index i, there are (variableCount - i) remaining variables.
            // therefore, it will have that many children
            int count = 0;
            for(int idx = 0; idx < recordNums.Count; idx++)
            {
                if (recordNums[idx])
                {
                    count += 1;
                }
            }
            ADNode adn = new ADNode(network.Size() - i, count);

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
                // create a vary node
                variables.Set(j, true);
                Varset newVariables = new Varset(variables);
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
                childNums[k] = childNums[k].Or(recordNums);
                childNums[k] = childNums[k].And(consistentRecords[i][k]);

                // also look for the mcv
                int count = 0;
                for (int idx = 0; idx < childNums[k].Count; idx++)
                {
                    if (childNums[k][idx])
                    {
                        count += 1;
                    }
                }
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
                int count = 0;
                for (int idx = 0; idx < childNums[k].Count; idx++)
                {
                    if (childNums[k][idx])
                    {
                        count += 1;
                    }
                }
                if (k == mcv || count == 0)
                {
                    continue;
                }

                ADNode child = MakeADTree(i + 1, childNums[k], depth + 1, variables);
                vn.SetChild(k, child);
            }

            return vn;

        }

        private int rMin;
        private BayesianNetwork network;
        private int recordCount;
        public int RecordCount
        {
            get
            {
                return RecordCount;
            }
        }
        private Varset zero;
        private ADNode root;
        private List<List<BitArray>> consistentRecords;
    }
}
