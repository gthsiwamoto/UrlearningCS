using System;
using Scoring;

namespace Main
{
    class MainClass
    {
        static void Main(string[] args)
        {
            ADTreeScoring ats = new ADTreeScoring();
            try
            {
                ats.Execute(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            //Console.In.ReadLine();
        }
    }
}
