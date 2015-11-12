using System.Collections;
using System.Collections.Generic;

namespace Scoring
{
    class ADNode
    {
        public ADNode(int size, int count)
        {
            this.count = count;
            for(int i = 0; i < size; i++)
            {
                children.Add(null);
            }
        }

        public void SetChild(int index, VaryNode child)
        {
            children[index] = child;
        }

        public VaryNode GetChild(int index)
        {
            return children[index];
        }

        private int count;
        public int Count
        {
            get
            {
                return count;
            }
        }
        private BitArray leafList = new BitArray(0);
        public BitArray LeafList
        {
            get
            {
                return leafList;
            }

            set
            {
                leafList = new BitArray(value);
            }
        }
        private List<VaryNode> children = new List<VaryNode>();
    }
}
