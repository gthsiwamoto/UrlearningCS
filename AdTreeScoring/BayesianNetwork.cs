using System.Collections.Generic;
using System.Collections;
using System;

namespace Datastructures
{
    class BayesianNetwork
    {
        public BayesianNetwork() { }

        public BayesianNetwork(RecordFile recordFile)
        {
            Initialize(recordFile);
        }

        private void Initialize(RecordFile recordFile)
        {
            // 変数名の設定
            for (int i = 0; i < recordFile.Header.Count; i++)
            {
                Variable v = new Variable(this, i);
                v.Name = recordFile.Header[i];
                nameToIndex[v.Name] = i;
                variables.Add(v);
            }

            // 値をセットする
            variables.ForEach(variable => variable.AddValues(recordFile));
        }

        public int Size()
        {
            return variables.Count;
        }

        public Variable Get(int variable)
        {
            return variables[variable];
        }

        public List<List<BitArray>> GetConsistentRecords(RecordFile recordFile)
        {
            List<List<BitArray>> consistentRecords = new List<List<BitArray>>();

            for(int i = 0; i < Size(); i++)
            {
                consistentRecords.Add(new List<BitArray>());
                int count = GetCardinality(i);

                for(int value = 0; value < count; value++)
                {
                    consistentRecords[i].Add(new BitArray(recordFile.Size()));
                }
            }

            for (int index = 0; index < recordFile.Size(); index++)
            {
                Record record = recordFile.Records[index];
                for (int variable = 0; variable < Size(); variable++)
                {
                    string v = record[variable];
                    int value = Get(variable).ValueToIndex[v];
                    consistentRecords[variable][value].Set(index, true);
                }
            }

            return consistentRecords;
        }

        public int GetCardinality(int variable)
        {
            return variables[variable].GetCardinality();
        }

        private List<Variable> variables = new List<Variable>();
        private Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
    }
}
