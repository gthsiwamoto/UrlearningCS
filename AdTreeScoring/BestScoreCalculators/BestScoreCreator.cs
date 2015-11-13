using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;
using Scoring;

namespace BestScoreCalculators
{
    class BestScoreCreator
    {
        public static List<BestScoreCalculator> Create(string type, ScoreCache cache)
        {
            List<BestScoreCalculator> spgs = new List<BestScoreCalculator>();
            for (int i = 0; i < cache.VariableCount; i++)
            {
                BestScoreCalculator spg = new BestScoreCalculator();
                if (type == "tree")
                {
                    // TODO SparseParentTreeの実装 
                }
                else if (type == "bitwise")
                {
                    // TODO SparseParentBitwiseの実装
                }
                else if (type == "list")
                {
                    spg = new SparseParentList(i, cache.VariableCount);
                }
                else
                {
                    throw new ArgumentException("Invalid BestScore Calculator type: '" + type + "'. Valid options are 'tree', 'bitwise' and 'list'.");
                }

                spg.Initialize(cache);
                spgs.Add(spg);
            }

            return spgs;
        }
        public static string BestScoreCalculatorString = "The data structure to use for BestScore calculations. [\"list\", \"tree\", \"bitwise\"]";
    }
}
