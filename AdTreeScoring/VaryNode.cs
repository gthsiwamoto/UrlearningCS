using System.Collections.Generic;

namespace AdTreeScoring
{
    class VaryNode
    {
        public VaryNode(int size)
        {
            for(int i = 0; i < size; i++)
            {
                children.Add(null);
            }
        }

        public void SetChild(int index, AdNode child)
        {
            children[index] = child;
        }

        public AdNode GetChild(int index)
        {
            return children[index];
        }

        private List<AdNode> children = new List<AdNode>();
        private int mcv;
        public int Mcv
        {
            get
            {
                return mcv;
            }

            set
            {
                mcv = value;
            }
        }
    }
}
