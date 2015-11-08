using System;
using System.Collections.Generic;

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

        public void Initialize(RecordFile recordFile)
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

        private List<Variable> variables = new List<Variable>();
        private Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
    }
}
