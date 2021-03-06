using System.Collections.Generic;
using System.Text;

namespace SharpPascal.Parsing
{
    internal static class ParserFactory
    {
        // A Parser that only returns a constant value
        public static Parser<T> Constant<T>(T value)
            => new Parser<T>(source => new ParseResult<T>(value, source));

        // A dummy Parser when forward declaration is necessary
        public static Parser<T> Forward<T>()
            => new Parser<T>(_ => throw new ParseException("Forward: Must implement Parse()"));

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
        public static Parser<string> Text(string text, bool ignoreCase = false)
            => new Parser<string>(source => source.Match(text, ignoreCase));

        // A Parser that matches zero or more occurrences
        public static Parser<List<T>> ZeroOrMore<T>(Parser<T> parser)
            => new Parser<List<T>>(source =>
                OneOrMore(parser).Parse(source)
                    ?? new ParseResult<List<T>>(new List<T>(), source));

        // A Parser that matches zero or more occurences
        public static Parser<string> ZeroOrMore(Parser<char> parser)
            => new Parser<string>(source =>
                OneOrMore(parser).Parse(source)
                    ?? new ParseResult<string>("", source));

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
        public static Parser<T?> Not<T>(Parser<T> parser) where T : class
            => new Parser<T?>(source =>
            {
                var result = parser.Parse(source);

                return result == null
                    ? new ParseResult<T?>(null, source)
                    : null;
            });

        // A Parser that returns a value only if the specified parser fails
        public static Parser<T?> SNot<T>(Parser<T> parser) where T : struct
            => new Parser<T?>(source =>
            {
                var result = parser.Parse(source);

                return result == null
                    ? new ParseResult<T?>(null, source)
                    : null;
            });

        // A Parser that optionally accept the rule
        public static Parser<T?> Maybe<T>(Parser<T> parser) where T : class
            => parser.Map(value => (T?)value).Or(Constant<T?>(null));
    }
}