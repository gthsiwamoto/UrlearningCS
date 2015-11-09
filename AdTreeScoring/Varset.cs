using System.Collections;

namespace Datastructures
{
    class Varset
    {
        public Varset(int size)
        {
            varset = new BitArray(size);
        }

        public Varset Set(int size, bool value = true)
        {
            varset.Set(size, value);
            return this;
        }

        private BitArray varset;
    }
}
