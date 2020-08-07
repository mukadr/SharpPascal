using System.Collections.Generic;
using System.Text;

namespace SharpPascal.Syntax.Parsing
{
    public static class ParserFactory
    {
        // A Parser that only returns a constant value
        public static Parser<T> Constant<T>(T value)
            => new Parser<T>(source => new ParseResult<T>(value, source));

        // A dummy Parser when forward declaration is necessary
        public static Parser<T> Forward<T>()
#pragma warning disable CS8604 // Possible null reference argument.
            => Not(Constant<T>(default)).OrError("Forward: Must implement Parse()");
#pragma warning restore CS8604 // Possible null reference argument.

        // A Parser that matches a single character
        public static Parser<char> Symbol(char c)
            => new Parser<char>(source => source.Match(c));

        // A Parser that matches a character between begin and end
        public static Parser<char> Symbol(char begin, char end)
            => new Parser<char>(source => source.Match(begin, end));

        // A Parser that matches anything until character c is found
        public static Parser<string> Until(char c)
            => new Parser<string>(source => source.MatchUntil(c));

        // A Parser that matches a string
        public static Parser<string> Text(string text)
            => new Parser<string>(source => source.Match(text));

        // A Parser that matches zero or more occurences
        public static Parser<List<T>> ZeroOrMore<T>(Parser<T> parser)
            => new Parser<List<T>>(source =>
            {
                var results = new List<T>();

                while (true)
                {
                    var result = parser.Parse(source);
                    if (result == null)
                    {
                        break;
                    }
                    results.Add(result.Value);
                    source = result.Source;
                }

                return new ParseResult<List<T>>(results, source);
            });

        // A Parser that matches zero or more occurences
        // Specialization for char parsers
        public static Parser<string> ZeroOrMore(Parser<char> parser)
            => new Parser<string>(source =>
            {
                var sb = new StringBuilder();

                while (true)
                {
                    var result = parser.Parse(source);
                    if (result == null)
                    {
                        break;
                    }

                    sb.Append(result.Value);
                    source = result.Source;
                }

                return new ParseResult<string>(sb.ToString(), source);
            });

        // A Parser that matches at least one occurence
        public static Parser<List<T>> OneOrMore<T>(Parser<T> parser)
            => new Parser<List<T>>(source =>
            {
                var result = parser.Parse(source);
                if (result == null)
                {
                    return null;
                }

                var results = new List<T>();

                while (true)
                {
                    results.Add(result.Value);
                    source = result.Source;

                    result = parser.Parse(source);
                    if (result == null)
                    {
                        break;
                    }
                }

                return new ParseResult<List<T>>(results, source);
            });

        // A Parser that matches at least one occurence
        // Specialization for char parsers
        public static Parser<string> OneOrMore(Parser<char> parser)
            => new Parser<string>(source =>
            {
                var result = parser.Parse(source);
                if (result == null)
                {
                    return null;
                }

                var sb = new StringBuilder();

                while (true)
                {
                    sb.Append(result.Value);
                    source = result.Source;

                    result = parser.Parse(source);
                    if (result == null)
                    {
                        break;
                    }
                }

                return new ParseResult<string>(sb.ToString(), source);
            });

        // A Parser that returns a value only if the specified parser fails
        public static Parser<T> Not<T>(Parser<T> parser)
            => new Parser<T>(source =>
            {
                var result = parser.Parse(source);
                if (result == null)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    return new ParseResult<T>(default, source);
#pragma warning restore CS8604 // Possible null reference argument.
                }
                return null;
            });

        // A Parser that optionally accept the rule
        public static Parser<T> Maybe<T>(Parser<T> parser)
#pragma warning disable CS8604 // Possible null reference argument.
            => parser.Or(Constant<T>(default));
#pragma warning restore CS8604 // Possible null reference argument.
    }
}