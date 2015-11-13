using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastructures
{
    class PriorityQueue
    {
        public PriorityQueue() { }

        public PriorityQueue(Comparer<Node> comparer)
        {
            this.comparer = comparer;
        }

        private int Compare(Node x, Node y)
        {
            if (comparer != null)
            {
                return comparer.Compare(x, y);
            }

            double diff = x.F - y.F;
            if (Math.Abs(diff) < Double.Epsilon)
            {
                return y.GetLayer() - x.GetLayer() > 0 ? 1 : -1;
            }
            return diff > 0 ? 1 : -1;
        }

        public void Push(Node x)
        {
            int i = heap.Count;
            heap.Add(null);

            while (i > 0)
            {
                // parent node number
                int p = (i - 1) / 2;

                if (Compare(heap[p], x) <= 0)
                {
                    break;
                }

                heap[i] = heap[p];
                i = p;
            }

            heap[i] = x;
        }

        public Node Pop()
        {
            Node ret = heap[0];
            heap.RemoveAt(0);

            if (heap.Count == 0)
            {
                return ret;
            }

            Node x = heap[heap.Count - 1];

            int i = 0;
            while (i * 2 + 1 < heap.Count)
            {
                // children
                int a = i * 2 + 1;
                int b = i * 2 + 2;

                if (b < heap.Count && Compare(heap[b], heap[a]) < 0)
                {
                    a = b;
                }

                if (Compare(heap[a], x) >= 0)
                {
                    break;
                }

                heap[i] = heap[a];
                i = a;
            }
            heap[i] = x;

            return ret;
        }

        public int Count()
        {
            return heap.Count;
        }

        public Node Peek()
        {
            return heap[0];
        }

        public bool Contains(Node x)
        {
            return heap.Contains(x);
        }

        public void Clear()
        {
            heap.Clear();
        }

        
        private Comparer<Node> comparer = null;
        private List<Node> heap = new List<Node>();
    }
}
