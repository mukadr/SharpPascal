namespace SharpPascal.Syntax.Parsing
{
    public class ParseResult<T>
    {
        // Value returned by the Parser
        public T Value { get; }

        // Source position updated after parsing
        public Source Source { get; }

        public ParseResult(T value, Source source)
        {
            Value = value;
            Source = source;
        }
    }
}
