using System;
using System.IO;
using static SharpPascal.Syntax.PascalParser;

namespace SharpPascal
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: spc { file ... fileN }");
                return;
            }

            foreach (var file in args)
            {
                var sourceText = File.ReadAllText(file);
                Parse(sourceText);
            }
        }
    }
}
