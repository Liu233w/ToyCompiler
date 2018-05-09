using System;
using LexicalAnalyzer;

namespace LexicalAnalyzerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            const string buffer = "cabd";

            var analyzer = new Simple();
            var res = analyzer.Analyze(buffer);

            Console.WriteLine(res);
        }
    }
}
