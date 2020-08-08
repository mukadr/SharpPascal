using System;

namespace SharpPascal.Syntax.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        { }
    }
}
