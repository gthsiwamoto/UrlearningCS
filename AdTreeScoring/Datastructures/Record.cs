using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastructures
{
    class Record : List<string>
    {
        public Record(string[] string_array) : base(string_array)
        {
        }

        public Record(int size) : base()
        {
            for (int i = 0; i < size; i++)
            {
                this.Add("");
            }
        }
    }
}
