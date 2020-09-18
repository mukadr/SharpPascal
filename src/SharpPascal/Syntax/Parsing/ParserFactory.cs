using System.Collections.Generic;

namespace SharpPascal.Syntax.Parsing
{
    internal static class ParserFactory
    {
        // A Parser that only returns a constant value
        public static Parser<T> Constant<T>(T value)
            => new Parser<T>(source => new ParseResult<T>(value, source));

        // A dummy Parser when forward declaration is necessary
        public static Parser<T> Forward<T>()
            => new Parser<T>((_) => throw new ParseException("Forward: Must implement Parse()"));

        // A Parser that matches a string
        public static Parser<string> Regex(string pattern, bool ignoreCase = false)
            => new Parser<string>(source => source.Match(pattern, ignoreCase));

        // A Parser that matches zero or more occurences
        public static Parser<List<T>> ZeroOrMore<T>(Parser<T> parser)
            => new Parser<List<T>>(source =>
                OneOrMore(parser).Parse(source)
                    ?? new ParseResult<List<T>>(new List<T>(), source));

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

        // A Parser that returns a value only if the specified parser fails
        public static Parser<T?> Not<T>(Parser<T> parser) where T: class
            => new Parser<T?>(source =>
            {
                var result = parser.Parse(source);
                if (result == null)
                {
                    return new ParseResult<T?>(null, source);
                }
                return null;
            });

        // A Parser that returns a value only if the specified parser fails
        public static Parser<T?> SNot<T>(Parser<T> parser) where T : struct
            => new Parser<T?>(source =>
            {
                var result = parser.Parse(source);
                if (result == null)
                {
                    return new ParseResult<T?>(null, source);
                }
                return null;
            });

        // A Parser that optionally accept the rule
        public static Parser<T?> Maybe<T>(Parser<T> parser) where T: class
            => parser.Map(value => (T?)value).Or(Constant<T?>(null));
    }
}