﻿using System.Collections.Generic;
using System.Collections;

namespace Datastructures
{
    class BayesianNetwork
    {
        public BayesianNetwork() { }
        public BayesianNetwork(int size)
        {
            variables = new List<Variable>();
            nameToIndex = new Dictionary<string, int>();
            for (int i = 0; i < size; i++)
            {
                Variable v = new Variable(this, i);
                string variableName = "Variable_" + i;
                v.Name = variableName;
                nameToIndex[v.Name] = variables.Count;
                variables.Add(v);
            }
        }

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

                if (recordFile.HasHeader)
                {
                    v.Name = recordFile.Header[i];
                }
                else
                {
                    string variableName = "Variable_" + i;
                    v.Name = variableName;
                }

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

        public Variable Get(string variable)
        {
            return variables[nameToIndex[variable]];
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

        public int GetVariableIndex(string variable)
        {
            return nameToIndex[variable];
        }

        public void FixCardinality()
        {
            for (int i = 0; i < variables.Count; i++)
            {
                variables[i].FixCardinality();
            }
        }

        public void SetParents(List<Varset> parents)
        {
            int i = 0;
            for(int k = 0; k < variables.Count; k++)
            {
                variables[k].SetParents(parents[i]);
                i += 1;
            }
            SetDefaultParentOrder();
        }

        public void SetDefaultParentOrder()
        {
            for (int i = 0; i < Size(); i++)
            {
                Get(i).SetDefaultParentOrder();
            }
        }

        public void SetUniformProbabilities()
        {
            UpdateParameterSize();
            for (int i = 0; i < Size(); i++)
            {
                Get(i).SetUniformProbabilities();
            }
        }

        public void UpdateParameterSize()
        {
            for (int i = 0; i < Size(); i++)
            {
                Get(i).UpdateParameterSize();
            }
        }

        public Variable AddVariable(string name)
        {
            nameToIndex[name] = variables.Count;
            Variable v = new Variable(this, variables.Count);
            v.Name = name;
            variables.Add(v);
            return v;
        }

        private List<Variable> variables = new List<Variable>();
        private Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
    }
}
