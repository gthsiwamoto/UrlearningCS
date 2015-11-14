using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;

namespace FileIO
{
    class HuginStructureWriter
    {
        public void Write(BayesianNetwork network, string fileName)
        {
            this.network = network;
            file = new StreamWriter(fileName, false);

            file.Write("net {}\n");
            for (int i = 0; i < network.Size(); i++)
            {
                WriteVariableDescription(i);
            }

            // write the parents
            for (int i = 0; i < network.Size(); i++)
            {
                if (network.Get(i).Parents.Cardinality() > 0)
                {
                    WriteNonrootVariable(i);
                }
                else
                {
                    WriteRootVariable(i);
                }
            }

            file.Close();

        }

        private void WriteVariableDescription(int index)
        {
            Variable v = network.Get(index);

            int cardinality = v.GetCardinality();
            string variableName = v.Name;

            file.Write("node " + variableName + " { \n states = ( ");

            // build the list of values
            for (int j = 0; j < cardinality; j++)
            {
                file.Write("\"" + v.GetValue(j) + "\" ");
            }

            file.Write(");\n");
            file.Write("}\n");


        }

        private void WriteRootVariable(int index)
        {
            Variable v = network.Get(index);

            file.Write("potential ( " + v.Name + " ) {\n");
            file.Write(" data = (");

            PrintProbabilities(v.Parameters, 0, v);

            file.Write(");\n");
            file.Write("}\n");
        }

        private void WriteNonrootVariable(int index)
        {
            Variable v = network.Get(index);
            Varset parents = v.Parents;

            file.Write("potential ( " + v.Name + " | ");

            PrintParents(parents);

            file.Write(" ) {\n");
            file.Write(" data = ");

            // now print the probabilities
            Record ins = v.GetFirstInstantiation();

            List<List<double>> parameters = v.Parameters;

            for (int i = 0; i < parents.Cardinality() + 1; i++)
            {
                file.Write("(");
            }

            for (int pIndex = 0; pIndex < v.Parameters.Count; pIndex++)
            {
                int pI = v.GetParentIndex(ins);

                PrintProbabilities(parameters, pI, v);

                int count = v.GetNextParentInstantiation(ins);
                for (int i = 0; i < count + 1; i++)
                {
                    file.Write(")");
                }
                for (int i = 0; i < count + 1; i++)
                {
                    file.Write("(");
                }
            }
            for (int i = 0; i < parents.Cardinality() + 1; i++)
            {
                file.Write(")");
            }

            file.Write(";\n");
            file.Write("}\n");
        }

        private void PrintParents(Varset parents)
        {
            for (int p = 0; p < network.Size(); p++)
            {
                if (parents.Get(p))
                {
                    file.Write(network.Get(p).Name);
                    file.Write(" ");
                }
            }
        }

        private void PrintInstantiation(Record instantiation, Varset parents)
        {
            // build up the list of parents
            string line = "";
            for (int p = 0; p < network.Size(); p++)
            {
                if (parents.Get(p))
                {
                    line += instantiation[p];
                    line += ",";
                }
            }
            //if (line.Length > 0)
            //{
            //    line = line.Substring(0, line.Length - 1);
            //}

            file.Write(line);
        }

        private void PrintProbabilities(List<List<double>> values, int pIndex, Variable v)
        {
            string s = "";
            for (int i = 0; i < v.GetCardinality(); i++)
            {
                s += values[pIndex][i];
                s += " ";
            }
            file.Write(s);
        }

        private BayesianNetwork network;
        private StreamWriter file;

    }
}
