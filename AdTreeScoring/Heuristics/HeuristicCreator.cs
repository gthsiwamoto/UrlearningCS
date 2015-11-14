using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datastructures;
using Scoring;
using BestScoreCalculators;

namespace Heuristics
{
    class HeuristicCreator
    {
        public static Heuristic CreateWithAncestors(string heuristicType, string argument, List<BestScoreCalculator> spgs, Varset ancestors, Varset scc)
        {
            Heuristic heuristic = new Heuristic();

            heuristicType = heuristicType.ToLower();

            if (heuristicType == "static")
            {
                int pdCount = int.Parse(argument);
                heuristic = new StaticPatternDatabase(spgs.Count, pdCount, false, ancestors, scc);
            }
            else if (heuristicType == "static_random")
            {
                // TODO
            }
            else if (heuristicType == "dynamic_optimal")
            {
                // TODO
            }
            else if (heuristicType == "dynamic")
            {
                // TODO
            }
            else if (heuristicType == "combined")
            {
                // TODO
            }
            else if (heuristicType == "file")
            {
                // TODO
            }
            else if (heuristicType == "simple")
            {
                // TODO
            }
            else
            {
                throw new ArgumentException("Invalid heuristic type: '" + heuristicType + "'. Valid options are 'static', 'static_randome', 'dynamic', 'dynamic_optimal', 'combined', 'file', 'simple' and 'Suzuki'");
            }

            heuristic.Initialize(spgs);
            return heuristic;
        }

        public static Heuristic Create(string heuristicType, string argument, List<BestScoreCalculator> spgs)
        {
            Varset ancestors = new Varset(spgs.Count);
            Varset scc = new Varset(spgs.Count);
            return CreateWithAncestors(heuristicType, argument, spgs, ancestors, scc);
        }

        private static string heuristicTypeString = "The type of heuristic to use. [\"static\", \"Static_random\", \"dynamic\", \"dynamic_optimal\", \"combined\", \"file\", \"simple\", \"Suzuki\"]";
        private static string heuristicArgumentString = "The argument for creating the heuristic, suche as number of pattern databases to use.";
    }

}
