namespace SharpPascal.Syntax.Parsing
{
    public class Source
    {
        // Source text being parsed
        public string Text { get; }

        // Current position to parse next grammar rule
        public int Position { get; }

        public Source(string text, int position = 0)
        {
            Text = text;
            Position = position;
        }

        // Matches the character c
        public ParseResult<char>? Match(char c)
        {
            if (Position < Text.Length &&
                Text[Position] == c)
            {
                var position = Position + 1;
                return new ParseResult<char>(Text[Position], new Source(Text, position));
            }

            return null;
        }

        // Matches a character between begin and end
        public ParseResult<char>? Match(char begin, char end)
        {
            if (Position < Text.Length &&
                Text[Position] >= begin &&
                Text[Position] <= end)
            {
                var position = Position + 1;
                return new ParseResult<char>(Text[Position], new Source(Text, position));
            }

            return null;
        }

        // Matches anything until last is found
        public ParseResult<string>? MatchUntil(char last)
        {
            if (Position < Text.Length)
            {
                var index = Text.IndexOf(last, Position);
                if (index > -1)
                {
                    var position = index + 1;
                    return new ParseResult<string>(Text.Substring(Position, position - Position), new Source(Text, position));
                }
            }
            return null;
        }

        // Matches the string s
        public ParseResult<string>? Match(string s)
        {
            if (Position + s.Length <= Text.Length &&
                Text.Substring(Position, s.Length) == s)
            {
                var position = Position + s.Length;
                return new ParseResult<string>(s, new Source(Text, position));
            }

            return null;
        }
    }
}
