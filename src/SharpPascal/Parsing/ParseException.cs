using System;

namespace SharpPascal.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        { }
    }
}
