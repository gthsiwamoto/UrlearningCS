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

        public void UpdateMetaInformation(string key, string value)
        {
            metaInformation[key] = value;
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
        Dictionary<string, string> metaInformation = new Dictionary<string, string>();
    }
}
