using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring;
using Datastructures;
using BestScoreCalculators;
using Heuristics;

namespace AStar
{
    using NodeMap = Dictionary<ulong, Node>;

    class AStarLearning
    {
        public static void Execute(string[] args)
        {
            // 引数のチェック
            if (args.Length != 1)
            {
                // エラーメッセージ表示
                Console.WriteLine("[エラー] 引数の数が不正です");
                Environment.Exit(0);
            }

            // オプションのチェック
            // 暫定的に初期値を代入
            string bestScoreCalculator = "list";
            string heuristicType = "static";
            string heuristicArgument = "2";
            string scoreFile = args[0];
            string netFile = "";
            string ancestorsArgument = "";
            string sccArgument = "";
            int runningTime = 0;

            // csvファイルの読み込み
            //RecordFile recordFile = new RecordFile();
            //recordFile.ReadRecord(inputFile, hasHeader, delimiter);
            //recordFile.Print();

            Console.WriteLine("URLerning A*");
            Console.WriteLine("Dataset: '" + scoreFile + "'");
            Console.WriteLine("Net file: '" + netFile + "'");
            Console.WriteLine("Best score calculator: '" + bestScoreCalculator + "'");
            Console.WriteLine("Heuristic type: '" + heuristicType + "'");
            Console.WriteLine("Heuristic argument: '" + heuristicArgument + "'");
            Console.WriteLine("Ancestors: '" + ancestorsArgument + "'");
            Console.WriteLine("SCC: '" + sccArgument + "'");

            Console.WriteLine("Reading score cache.");
            ScoreCache cache = new ScoreCache();
            cache.Read(scoreFile);
            int variableCount = cache.VariableCount;

            // parse the ancestor and scc variables
            Varset ancestors = new Varset(variableCount);
            Varset scc = new Varset(variableCount);

            // if no sc was specified, assume we just want to learn everything
            if (sccArgument == "")
            {
                scc.SetAll(true);
            }
            else
            {
                scc.SetFromCsv(sccArgument);
                ancestors.SetFromCsv(ancestorsArgument);
            }

            Console.WriteLine("Creating BestScore Calculators.");
            List<BestScoreCalculator> spgs = BestScoreCreator.Create(bestScoreCalculator, cache);

            Console.WriteLine("Creating heuristic.");
            Heuristic heuristic = HeuristicCreator.CreateWithAncestors(heuristicType, heuristicArgument, spgs, ancestors, scc);

            NodeMap generatedNodes = new NodeMap();

            PriorityQueue openList = new PriorityQueue();

            byte leaf = 0;
            Node root = new Node(0, 0, ancestors, leaf);
            openList.Push(root);

            Console.Write("The start is ");
            ancestors.Print();

            bool c = false;
            double lb = heuristic.h(ancestors, c);

            Console.WriteLine("The lb is " + lb);

            Node goal = null;
            Varset allVariables = new Varset(ancestors);
            allVariables = allVariables.Or(scc);

            double upperBound = Double.MaxValue;
            bool complete = false;
            bool outOfTime = false;

            Console.WriteLine("Beginning search");
            int nodesExpanded = 0;
            while (openList.Count() > 0 && !outOfTime)
            {
                Node u = openList.Pop();
                nodesExpanded++;

                Varset variables = u.SubNetwork;

                // check if it is the goal
                if (variables.ToULong() == allVariables.ToULong())
                {
                    goal = u;
                    break;
                }

                if (u.F > upperBound)
                {
                    break;
                }

                // note that it is in the closed list
                u.PqPos = -2;

                // expand
                for (leaf = 0; leaf < variableCount; leaf++)
                {
                    // make sure this variable was not already present
                    if (variables.Get(leaf))
                    {
                        continue;
                    }

                    // also make sure this is one of the variables we care about
                    if (!scc.Get(leaf))
                    {
                        continue;
                    }

                    // get the new variable set
                    Varset newVariables = new Varset(variables);
                    newVariables.Set(leaf, true);

                    Node succ = generatedNodes.ContainsKey(newVariables.ToULong()) ? generatedNodes[newVariables.ToULong()] : null;

                    // check if this is the first time we have generated this node
                    double g;
                    if (succ == null)
                    {
                        // get the cost along this path
                        Console.Write("About to check for using leaf '" + leaf + "', newVariables: ");
                        newVariables.Print();

                        g = u.G + spgs[leaf].GetScore(newVariables);

                        // calculate the heuristic estimate
                        complete = false;
                        double h = heuristic.h(newVariables, complete);

                        Console.WriteLine("g: " + g + "h: " + h);

                        // update all the values
                        succ = new Node(g, h, newVariables, leaf);

                        // add it to the open list
                        openList.Push(succ);

                        // and to the list of generated nodes
                        generatedNodes[newVariables.ToULong()] = succ;
                        continue;
                    }

                    // assume the heuristic is consistent
                    // so check if it was in the closed list
                    if (succ.PqPos == -2)
                    {
                        continue;
                    }

                    // so we have generated a node in the open list
                    // see if the new path is better
                    g = u.G + spgs[leaf].GetScore(variables);
                    if (g < succ.G)
                    {
                        // the update the information
                        succ.Leaf = leaf;
                        succ.G = g;

                        // and the priority queue
                        // openList.update(succ);
                    }
                }
            }

            Console.WriteLine("Nodes expanded: " + nodesExpanded + ", open list size: " + openList.Count());

            heuristic.PrintStatistics();

            if (goal != null)
            {
                Console.WriteLine("Found solution: ", goal.F);

                if (netFile.Length > 0)
                {
                    BayesianNetwork network = cache.Network;
                    // TODO
                }
            }
            else
            {
                Node u = openList.Pop();
                Console.WriteLine("No solution found.");
                Console.WriteLine("Lower bound: ", u.F);
            }
        }
    }
}
