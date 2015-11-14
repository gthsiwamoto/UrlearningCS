using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastructures
{
    class Node
    {
        public Node()
        {
            g = 0;
            h = 0;
            subNetwork = new Varset(0);
            leaf = 0;
            pqPos = -1;
        }

        public Node(double g, double h, Varset subNetwork, byte leaf)
        {
            this.g = g;
            this.h = h;
            this.subNetwork = new Varset(subNetwork);
            this.leaf = leaf;
            pqPos = 0;
        }

        public Node(Node n)
        {
            g = n.g;
            h = n.h;
            leaf = n.leaf;
            subNetwork = new Varset(n.subNetwork);
        }

        public byte GetLayer()
        {
            return (byte)subNetwork.Cardinality();
        }

        public double F
        {
            get
            {
                return g + h;
            }
        }

        protected double g;
        public double G
        {
            get
            {
                return g;
            }

            set
            {
                g = value;
            }
        }
        protected double h;
        public double H
        {
            get
            {
                return h;
            }

            set
            {
                h = value;
            }
        }
        protected Varset subNetwork;
        public Varset SubNetwork
        {
            get
            {
                return subNetwork;
            }

            set
            {
                subNetwork = new Varset(value);
            }
        }
        protected byte leaf;
        public byte Leaf
        {
            get
            {
                return leaf;
            }

            set
            {
                leaf = value;
            }
        }
        protected int pqPos;
        public int PqPos
        {
            get
            {
                return pqPos;
            }

            set
            {
                pqPos = value;
            }
        }
    }
}
