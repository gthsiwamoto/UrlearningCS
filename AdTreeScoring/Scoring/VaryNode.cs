using System.Collections.Generic;

namespace Scoring
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

        public void SetChild(int index, ADNode child)
        {
            children[index] = child;
        }

        public ADNode GetChild(int index)
        {
            return children[index];
        }

        private List<ADNode> children = new List<ADNode>();
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
