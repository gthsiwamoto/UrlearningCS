using System;
using System.Collections.Generic;

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
            recordFile.Record.ForEach(line => AddValue(line[index]));
        }

        public void AddValue(string value)
        {
            if (!valueToIndex.ContainsKey(value))
            {
                valueToIndex[value] = GetCardinality();
                values.Add(value);
            }

        }

        public int GetCardinality()
        {
            return values.Count;
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
        private BayesianNetwork network;
        private Dictionary<string, int> valueToIndex = new Dictionary<string, int>(); 
        private List<string> values = new List<string>();
    }
}
