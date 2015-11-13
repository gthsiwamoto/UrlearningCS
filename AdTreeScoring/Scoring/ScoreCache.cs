using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;
    class ScoreCache
    {
        public ScoreCache() { }

        public void Read(string fileName)
        {
            network = new BayesianNetwork(); 
            StreamReader sr;
            sr = new StreamReader(fileName);

            List<string> tokens = new List<string>();
            string line = "";

            // read meta information until we hit the first variable
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();

                // skip empty lines and comments
                if (line.Length == 0 || line.Substring(0, 1) == "#")
                {
                    continue;
                }

                // check if we reached the first variable
                if (line.Contains("VAR "))
                {
                    break;
                }

                // make sure this is a meta line
                if (!line.Contains("META"))
                {
                    throw new FormatException("Error while parsing META information of network. Expected META line or Variable. Line: '" + line + "'");
                }

                tokens = ParseMetaInformation(line);

                if (tokens.Count != 2)
                {
                    throw new FormatException("Error while parsing META information of network. Too many tokens. Line: '" + line + "'");
                }

                tokens[0] = tokens[0].Trim();
                tokens[1] = tokens[1].Trim();
                UpdateMetaInformation(tokens[0], tokens[1]);
            }

            // line currently points to a variable name
            tokens = Parse(line, 0, " ");
            Variable v = network.AddVariable(tokens[1]);

            // read in the variable names
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();

                // skip empty lines and comments
                if (line.Length == 0 || line.Substring(0, 1) == "#")
                {
                    continue;
                }

                if (line.Contains("META"))
                {
                    tokens = ParseMetaInformation(line);

                    if (tokens[0].Contains("arity"))
                    {
                        v.SetArity(int.Parse(tokens[1]));
                    }
                    else if (tokens[0].Contains("values"))
                    {
                        List<string> values = ParseVariableValues(tokens[1]);
                        v.SetValues(values);
                    }
                    else
                    {
                        tokens[0] = tokens[0].Trim();
                        tokens[1] = tokens[1].Trim();
                    }
                }

                if (line.Contains("VAR "))
                {
                    tokens = Parse(line, 0, " ");
                    v = network.AddVariable(tokens[1]);
                }
            }

            sr.Close();
            SetVariableCount(network.Size());

            // now that we have the variable names, read in the parent sets
            sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();

                if (line.Length == 0 || line.Substring(0, 1) == "#" || line.Contains("META"))
                {
                    continue;
                }

                tokens = Parse(line, 0, " ");
                if (line.Contains("VAR "))
                {
                    v = network.Get(tokens[1]);
                    continue;
                }

                // then parse the score for the current variable
                Varset parents = new Varset(network.Size());
                double score = -1 * double.Parse(tokens[0]); // multiply by -1 to minimize

                for (int i = 1; i < tokens.Count; i++)
                {
                    int index = network.GetVariableIndex(tokens[i]);
                    parents.Set(index, true);
                }
                PutScore(v.Index, parents, score);
            }
            sr.Close();
            
        }

        public void PutScore(int variable, Varset parents, double score)
        {
            cache[variable][parents.ToULong()] = score;
        }

        public void SetVariableCount(int variableCount)
        {
            this.variableCount = variableCount;
            for (int i = 0; i < variableCount; i++)
            {
                cache.Add(new DoubleMap());
            }
        }

        public void UpdateMetaInformation(string key, string value)
        {
            metaInformation[key] = value;
        }

        private static List<string> ParseMetaInformation(string line)
        {
            return Parse(line, 4, "=");
        }

        private static List<string> ParseVariableValues(string line)
        {
            return Parse(line, 0, ",");
        }

        private static List<string> Parse(string line, int start, string delimiters)
        {
            List<string> tokens = new List<string>();
            string trimmedLine = line.Substring(start);
            trimmedLine = trimmedLine.Trim();
            string[] delimiterArray = { delimiters };
            tokens.AddRange(trimmedLine.Split(delimiterArray, StringSplitOptions.RemoveEmptyEntries));
            return tokens;
        }

        public DoubleMap GetCache(int variable)
        {
            return cache[variable];
        }

        private int variableCount;
        public int VariableCount
        {
            get
            {
                return variableCount;
            }
        }
        private BayesianNetwork network;
        private List<int> variableCardinalities = new List<int>();
        private List<DoubleMap> cache = new List<DoubleMap>();
        private Dictionary<string, string> metaInformation = new Dictionary<string, string>();

    }
}
