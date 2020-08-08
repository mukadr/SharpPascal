using System.Linq;

namespace SharpPascal.Syntax.Parsing
{
    public class Source
    {
        // Source text being parsed
        public string Text { get; }

        // Current position to parse next grammar rule
        public int Position { get; }

        // Current line number
        public int Line { get; }

        public Source(string text, int position = 0, int line = 1)
        {
            Text = text;
            Position = position;
            Line = line;
        }

        // Matches the character c
        public ParseResult<char>? Match(char c)
        {
            if (Position < Text.Length &&
                Text[Position] == c)
            {
                var position = Position + 1;
                var line = Line + (Text[Position] == '\n' ? 1 : 0);
                return new ParseResult<char>(Text[Position], new Source(Text, position, line));
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
                var line = Line + (Text[Position] == '\n' ? 1 : 0);
                return new ParseResult<char>(Text[Position], new Source(Text, position, line));
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
                    var s = Text.Substring(Position, position - Position);
                    var line = Line + s.Count(c => c == '\n');
                    return new ParseResult<string>(s, new Source(Text, position, line));
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
                var line = Line + s.Count(c => c == '\n');
                return new ParseResult<string>(s, new Source(Text, position, line));
            }

            return null;
        }
    }
}
