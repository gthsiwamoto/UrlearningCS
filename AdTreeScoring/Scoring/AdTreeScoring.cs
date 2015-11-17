using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datastructures;

namespace Scoring
{
    using DoubleMap = Dictionary<ulong, double>;

    class ADTreeScoring
    {
        public ADTreeScoring() { }

        public static void Execute(string[] args)
        {
            // 引数のチェック
            //if (args.Length != 2)
            //{
            //    // エラーメッセージ表示
            //    Console.WriteLine("[エラー] 引数の数が不正です");
            //    Environment.Exit(0);
            //}

            // オプションのチェック
            // 暫定的に初期値を代入
            int rMin = 5; // The minimum number of records in the AD-tree nodes.
            char delimiter = ',';
            bool hasHeader = false;
            string sf = "suzuki";
            int maxParents = 3;
            string constraintsFile = "";
            int runningTime = -1;
            int threadCount = 1;
            bool prune = true;
            string basename = "autos";
            string inputFile = "records/" + basename + ".csv";
            string outputFile = basename + ".output";
            double equivarentSampleSize = 1;

            // csvファイルの読み込み
            RecordFile recordFile = new RecordFile();
            recordFile.ReadRecord(inputFile, hasHeader, delimiter);
            //recordFile.Print();

            // BayesianNetworkの初期化
            BayesianNetwork network = new BayesianNetwork(recordFile);

            // AD-Treeの初期化
            ADTree adTree = new ADTree(rMin, network, recordFile);

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
            else if (sf == "suzuki") { }
            else
            {
                throw new ArgumentException("Invalid scoring function. Options are: 'BIC', 'fNML', 'BDeu' or 'Suzuki'.");
            }

            // TODO constraintsの実装
            Constraints constraints = null;
            if (constraintsFile.Length > 0)
            {
                constraints = Constraints.ParseConstraints(constraintsFile, network);
            }

            ScoringFunction scoringFunction = null;

            List<double> ilogi = LogLikelihoodCalculator.GetLogCache(recordFile.Size());
            LogLikelihoodCalculator llc = new LogLikelihoodCalculator(adTree, network, ilogi);

            // TODO regretの実装
            //std::vector<std::vector<float>*>* regret = scoring::getRegretCache(recordFile.size(), network.getMaxCardinality());

            if (sf == "bic")
            {
                scoringFunction = new BICScoringFunction(network, recordFile, llc, constraints);
            }
            else if (sf == "fnml")
            {
                //
            }
            else if (sf == "bdeu")
            {
                scoringFunction = new BDeuScoringFunction(equivarentSampleSize, network, adTree, constraints);
            }
            else if (sf == "suzuki")
            {
                // TODO constraintsを反映する
                scoringFunction = new SuzukiScoringFunction(network, adTree);
            }

            ScoreCalculator scoreCalculator = new ScoreCalculator(scoringFunction, maxParents, network.Size(), runningTime, constraints);
            // TODO Parallelクラスによる並列処理
            Parallel.For(0, threadCount, i =>
            {
                for (int variable = 0; variable < network.Size(); variable++)
                {
                    if(variable % threadCount != i)
                    {
                        continue;
                    }

                    Console.WriteLine("Thread: " + i + ", Variable: " + variable + ", Time: " + DateTime.Now);

                    DoubleMap sc = new DoubleMap();
                    scoreCalculator.CalculateScores(variable, sc);

                    int size = sc.Count;
                    Console.WriteLine("Thread: " + i + ", Variable: " + variable + ", Size before pruning: " + size + ", Time: " + DateTime.Now);

                    if (prune)
                    {
                        scoreCalculator.Prune(sc);
                        int prunedSize = sc.Count;
                        Console.WriteLine("Thread: " + i + ", Variable: " + variable + ", Size after pruning: " + prunedSize + ", Time: " + DateTime.Now);
                    }

                    string varFilename = outputFile + "." + variable;
                    StreamWriter writer = new StreamWriter(varFilename, false);

                    Variable var = network.Get(variable);
                    writer.Write("VAR " + var.Name + "\n");
                    writer.Write("META arity=" + var.GetCardinality() + "\n");
                    writer.Write("META values=");
                    for (int k = 0; k < var.GetCardinality(); k++)
                    {
                        writer.Write(var.GetValue(k) + " ");
                    }
                    writer.Write("\n");

                    foreach (KeyValuePair<ulong, double> kvp in sc)
                    {
                        Varset parentSet = new Varset(kvp.Key);
                        double s = kvp.Value;

                        writer.Write(s + " ");

                        for (int p = 0; p < network.Size(); p++)
                        {
                            if (parentSet.Get(p))
                            {
                                writer.Write(network.Get(p).Name + " ");
                            }
                        }
                        writer.Write("\n");
                    }
                    writer.Write("\n");
                    writer.Close();
                }
            });

            // concatenate all of the files together
            StreamWriter outFile = new StreamWriter(outputFile, false); // 修正が必要かも
            // first, the header information
            string header = "META pss_version = 0.1\nMETA input_file=" + inputFile + "\nMETA num_records=" + recordFile.Size() + "\n";
            header += "META parent_limit=" + maxParents + "\nMETA score_type=" + sf + "\nMETA ess=" + equivarentSampleSize + "\n\n";

            outFile.Write(header);

            for (int variable = 0; variable < network.Size(); variable++)
            {
                string varFilename = outputFile + "." + variable;
                StreamReader reader = new StreamReader(varFilename);
                outFile.Write(reader.ReadToEnd());
                reader.Close();
            }
            outFile.Close();
        }
    }
}
