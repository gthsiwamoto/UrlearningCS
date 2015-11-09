using System;
using Datastructures;

namespace Scoring
{
    class AdTreeScoring
    {
        public AdTreeScoring() { }

        public void Execute(string[] args)
        {
            // 引数のチェック
            if (args.Length != 2)
            {
                // エラーメッセージ表示
                Console.WriteLine("[エラー] 引数の数が不正です");
                Environment.Exit(0);
            }

            // オプションのチェック
            // 暫定的に初期値を代入
            int rMin = 5; // The minimum number of records in the AD-tree nodes.
            char delimiter = ',';
            bool hasHeader = true;
            string sf = "BIC";
            int maxParents = 0;

            // csvファイルの読み込み
            RecordFile recordFile = new RecordFile();
            recordFile.ReadRecord(args[0], hasHeader, delimiter);
            //recordFile.Print();

            // BayesianNetworkの初期化
            BayesianNetwork network = new BayesianNetwork(recordFile);

            // AD-Treeの初期化
            AdTree adTree = new AdTree(rMin, network, recordFile);

            // scoring functionの設定
            sf = sf.ToLower();
            if (maxParents > network.Size() || maxParents < 1)
            {
                maxParents = network.Size() - 1;
            }

            if (sf == "bic")
            {
                int maxParentCount = (int)Math.Log(2 * recordFile.Size() / Math.Log(recordFile.Size()));
                if(maxParentCount < maxParents)
                {
                    maxParents = maxParentCount;
                }
            }
            else if (sf == "fnml") { }
            else if (sf == "bdeu") { }
            else
            {
                throw new ArgumentException("Invalid scoring function. Options are: 'BIC', 'fNML' or 'BDeu'.");
            }

            // TODO constraintsの部分
            //scoring::Constraints* constraints = NULL;
            //if (constraintsFile.length() > 0)
            //{
            //    constraints = scoring::parseConstraints(constraintsFile, network);
            //}

            ScoringFunction scoringFunction = null;
        }
    }
}
