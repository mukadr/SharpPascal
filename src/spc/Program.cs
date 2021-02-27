using SharpPascal;
using System;
using System.IO;

namespace spc
{
    internal static class Program
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
                new PascalParser().Parse(sourceText);
            }
        }
    }
}
