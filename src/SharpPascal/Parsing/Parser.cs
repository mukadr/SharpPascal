using System;
using static SharpPascal.Parsing.ParserFactory;

namespace SharpPascal.Parsing
{
    internal class Parser<T>
    {
        public Func<Source, ParseResult<T>?> Parse { get; set; }

        public Parser(Func<Source, ParseResult<T>?> parse)
        {
            Parse = parse;
        }

        public T ParseToCompletion(string text)
            => ParseToCompletion(new Source(text));

        private T ParseToCompletion(Source source)
        {
            ParseResult<T>? result = null;

            try
            {
                result = Parse(source);

                if (result == null ||
                    result.Source.Location.Position != source.Text.Length)
                {
                    throw new ParseException("Parse Error: expected end of source");
                }

                return result.Value;
            }
            catch (ParseException ex)
            {
                var line = result?.Source.Location.Line ?? 1;
                throw new ParseException(line + ": " + ex.Message);
            }
        }

        // Tries another parser if this fails
        public Parser<T> Or(Parser<T> other)
            => new Parser<T>(source => Parse(source) ?? other.Parse(source));

        // Calls the next callback passing the parsed value
        public Parser<U> Bind<U>(Func<T, Parser<U>> next)
            => new Parser<U>(source =>
            {
                var result = Parse(source);
                return result != null
                    ? next(result.Value).Parse(result.Source)
                    : null;
            });

        // Calls the next callback passing the parsed value and location
        public Parser<U> Bind<U>(Func<T, Location, Parser<U>> next)
            => new Parser<U>(source =>
            {
                var result = Parse(source);
                return result != null
                    ? next(result.Value, result.Source.Location).Parse(result.Source)
                    : null;
            });

        // Tries another parser if this succeeds
        public Parser<U> And<U>(Parser<U> other)
            => Bind(_ => other);

        // Maps the parsed value with the map function
        public Parser<U> Map<U>(Func<T, U> map)
            => Bind(value => Constant(map(value)));

        // Maps the parsed value and location with the map function
        public Parser<U> Map<U>(Func<T, Location, U> map)
            => Bind((value, location) => Constant(map(value, location)));

        // Executes next parser, ignoring its result
        public Parser<T> Skip<U>(Parser<U> next)
            => new Parser<T>(source =>
            {
                var result = Parse(source);
                if (result == null)
                {
                    return result;
                }

                var consumed = next.Parse(result.Source);
                return consumed == null
                    ? result
                    : new ParseResult<T>(result.Value, consumed.Source);
            });
    }
}
