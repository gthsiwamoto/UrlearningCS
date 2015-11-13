using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scoring;
using Datastructures;
using BestScoreCalculators;

namespace AStar
{
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

            Console.WriteLine("URLerning A*\n");
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
            Varset ancestors = new Varset(0);
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
        }

    }
}
