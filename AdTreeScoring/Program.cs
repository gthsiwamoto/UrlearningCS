using System;
using AdTreeScoring;

namespace AdTreeScoring
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdTreeScoring ats = new AdTreeScoring();
            ats.Execute(args);
            //Console.In.ReadLine();
        }
    }
}
