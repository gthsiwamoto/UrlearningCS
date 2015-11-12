using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring
{
    class ContingencyTableNode
    {
        public ContingencyTableNode(int value, int childrenSize, int leafCount)
        {
            this.value = value;
            this.leafCount = leafCount;

            for (int i = 0; i < childrenSize; i++)
            {
                children.Add(null);
            }
        }

        public ContingencyTableNode GetChild(int index)
        {
            return children[index];
        }

        public void SetChild(int index, ContingencyTableNode child)
        {
            children[index] = child;
        }

        public void Subtract(ContingencyTableNode other)
        {
            if (IsLeaf())
            {
                value -= other.value;
                return;
            }

            for (int k = 0; k < children.Count; k++)
            {
                if (children[k] == null || other.children[k] == null)
                {
                    continue;
                }
                children[k].Subtract(other.children[k]);
            }
        }

        public bool IsLeaf()
        {
            return children.Count == 0;
        }

        private int value;
        public int Value
        {
            get
            {
                return value;
            }
        }
        private int leafCount;
        public int LeafCount
        {
            get
            {
                return leafCount;
            }

            set
            {
                leafCount = value;
            }
        }
        private List<ContingencyTableNode> children = new List<ContingencyTableNode>();
    }
}
