using System;
using Datastructures;

namespace AdTreeScoring
{
    class MainClass
    {
        static void Main(string[] args)
        {
            // 引数のチェック
            if (args.Length != 2)
            {
                // エラーメッセージ表示
                Console.WriteLine("[エラー] 引数の数が不正です");
                Environment.Exit(0);
            }

            // csvファイルの読み込み
            RecordFile recordFile = new RecordFile();
            recordFile.ReadRecord(args[0]);

            BayesianNetwork network = new BayesianNetwork();
            network.Initialize(recordFile);


            //Console.In.ReadLine();
        }
    }
}
