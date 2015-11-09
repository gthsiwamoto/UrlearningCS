using System;
using AdTreeScoring;

namespace Scoring
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdTreeScoring ats = new AdTreeScoring();
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
