using System;
using System.Collections.Generic;
using Datastructures;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring
{
    class LogLikelihoodCalculator
    {
        public LogLikelihoodCalculator(ADTree adTree, BayesianNetwork network, List<double> ilogi)
        {
            Initialize(adTree, network, ilogi);
        }

        public static List<double> GetLogCache(int recordCount)
        {
            List<double> logCache = new List<double>();
            logCache.Add(0.0);
            for (int i = 1; i < recordCount + 2; i++)
            {
                double l = i * Math.Log(i);
                logCache.Add(l);
            }
            return logCache;
        }

        private void Initialize(ADTree adTree, BayesianNetwork network, List<double> ilogi)
        {
            this.adTree = adTree;
            this.network = network;
            this.ilogi = ilogi;
        }

        private ADTree adTree;
        private BayesianNetwork network;
        private List<double> ilogi = new List<double>();
    }
}
