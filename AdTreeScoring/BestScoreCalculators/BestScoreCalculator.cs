using Datastructures;
using Scoring;

namespace BestScoreCalculators
{
    class BestScoreCalculator
    {
        public virtual double GetScore(Varset pars)
        {
            return 0;
        }

        public virtual double GetScore(int index)
        {
            return 0;
        }

        public virtual Varset GetParents(int index)
        {
            return null;
        }

        public virtual Varset GetParents()
        {
            return null;
        }

        public virtual void Initialize(ScoreCache scoreCache) { }

        public virtual int Size()
        {
            return 0;
        }

        public virtual void Print() { }

    }
}
