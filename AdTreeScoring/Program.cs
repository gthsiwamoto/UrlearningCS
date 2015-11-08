using System;
using DataStructures;

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
            recordFile.readRecord(args[0]);
            recordFile.print();

            Console.In.ReadLine();
        }
    }
}
