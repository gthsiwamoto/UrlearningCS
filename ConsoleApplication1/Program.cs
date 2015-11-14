using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStar;

namespace Main
{
    class MainClass
    {
        static void Main(string[] args)
        {
            try
            {
                AStarLearning.Execute(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.In.ReadLine();
        }
    }
}
