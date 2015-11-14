using System;
using Scoring;
using AStar;

namespace Main
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AStarLearning asl = new AStarLearning();
            //ADTreeScoring.Execute(args);
            asl.Execute(args);
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //Console.In.ReadLine();
        }
    }
}
