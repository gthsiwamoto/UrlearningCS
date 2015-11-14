using System.Collections.Generic;
using System;

namespace Datastructures
{
    class Variable
    {
        public Variable(BayesianNetwork network, int index)
        {
            this.index = index;
            this.network = network;
            parents = new Varset(network.Size());
        }

        public void AddValues(RecordFile recordFile)
        {
            recordFile.Records.ForEach(line => AddValue(line[index]));
        }

        public void AddValue(string value)
        {
            if (!valueToIndex.ContainsKey(value))
            {
                valueToIndex[value] = GetCardinality();
                values.Add(value);
            }
        }

        public string GetValue(int index)
        {
            return values[index];
        }

        public int GetCardinality()
        {
            return values.Count;
        }

        public void SetArity(int arity)
        {
            values.Clear();
            for (int i = 0; i < arity; i++)
            {
                values.Add("Value_" + i);
            }
        }

        public void SetValues(List<string> values)
        {
            this.values.Clear();
            this.valueToIndex.Clear();
            for (int i = 0; i < values.Count; i++)
            {
                AddValue(values[i]);
            }
        }

        public void SetParents(Varset parents)
        {
            this.parents = new Varset(parents);
        }

        public void SetDefaultParentOrder()
        {
            parentOrder.Clear();
            for (int i = 0; i < network.Size(); i++)
            {
                if (parents.Get(i))
                {
                    parentOrder.Add(i);
                }
            }
        }

        public void UpdateParameterSize()
        {
            int count = 1;
            for (int p = 0; p < network.Size(); p++)
            {
                if (parents.Get(p))
                {
                    count *= network.Get(p).GetCardinality();
                }
            }

            parameters.Clear();
            for (int i = 0; i < count; i++)
            {
                List<double> p = new List<double>();
                for (int j = 0; j < GetCardinality(); j++)
                {
                    p.Add(0);
                }
                parameters.Add(p);
            }
        }

        public void SetUniformProbabilities()
        {
            double uniformValue = 1.0 / GetCardinality();
            for (int i = 0; i < parameters.Count; i++)
            {
                for (int k = 0; k < GetCardinality(); k++)
                {
                    parameters[i][k] = uniformValue;
                }
            }
        }

        public void UpdateMetaInformation(string key, string value)
        {
            metaInformation[key] = value;
        }

        public void FixCardinality()
        {
            if (values.Count == 0)
            {
                AddValue("0");
            }

            if (values.Count == 1)
            {
                AddValue("1");
            }
        }

        public Record GetFirstInstantiation()
        {
            Record ins = new Record(network.Size());

            for (int i = 0; i < network.Size(); i++)
            {
                if (parents.Get(i))
                {
                    ins[i] = network.Get(i).GetValue(0);
                }
            }
            return ins;
        }

        public int GetParentIndex(Record instantiation)
        {
            int total = 1;
            int pIndex = 0;

            // for each parent
            for (int i = 0; i < parentOrder.Count; i++)
            {
                int p = parentOrder[i];
                Variable parent = network.Get(p);
                string pValue = instantiation[p];
                pIndex += parent.ValueToIndex[pValue] * total;
                total *= parent.GetCardinality();
            }
            return pIndex;
        }

        public int GetNextParentInstantiation(Record instantiation)
        {
            int count = 0;
            for (int p = parents.LastSetBit(); p > -1; p = parents.PreviousSetBit(p))
            {
                string value = instantiation[p];
                int i = network.Get(p).ValueToIndex[value];
                if (i < network.GetCardinality(p) - 1)
                {
                    string newValue = network.Get(p).GetValue(i + 1);
                    instantiation[p] = newValue;
                    return count;
                }
                else
                {
                    string newValue = network.Get(p).GetValue(0);
                    instantiation[p] = newValue;
                    count++;
                }
            }
            return -1;
        }


        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }
        private BayesianNetwork network;
        private Dictionary<string, int> valueToIndex = new Dictionary<string, int>(); 
        public Dictionary<string, int> ValueToIndex
        {
            get
            {
                return valueToIndex;
            }
        }
        private List<string> values = new List<string>();
        private Dictionary<string, string> metaInformation = new Dictionary<string, string>();
        private Varset parents;
        public Varset Parents
        {
            get
            {
                return parents;
            }
        }
        private List<int> parentOrder = new List<int>();
        private List<List<double>> parameters = new List<List<double>>();
        public List<List<double>> Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}
