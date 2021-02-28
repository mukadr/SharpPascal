using System;
using System.Linq;

namespace SharpPascal.Parsing
{
    internal class Source
    {
        // Source text being parsed
        public string Text { get; }

        // Current location
        public Location Location { get; }

        public Source(string text, Location? location = null)
        {
            Text = text;
            Location = location ?? new Location(0, 1);
        }

        // Matches the character c
        public ParseResult<char>? Match(char c)
        {
            if (Location.Position < Text.Length &&
                Text[Location.Position] == c)
            {
                var position = Location.Position + 1;
                var line = Location.Line + (Text[Location.Position] == '\n' ? 1 : 0);
                return new ParseResult<char>(Text[Location.Position], new Source(Text, new Location(position, line)));
            }

            return null;
        }

        // Matches a character between begin and end
        public ParseResult<char>? Match(char begin, char end)
        {
            if (Location.Position < Text.Length &&
                Text[Location.Position] >= begin &&
                Text[Location.Position] <= end)
            {
                var position = Location.Position + 1;
                var line = Location.Line + (Text[Location.Position] == '\n' ? 1 : 0);
                return new ParseResult<char>(Text[Location.Position], new Source(Text, new Location(position, line)));
            }

            return null;
        }

        // Matches anything until last is found
        public ParseResult<string>? MatchUntil(char last)
        {
            if (Location.Position < Text.Length)
            {
                var index = Text.IndexOf(last, Location.Position);
                if (index > -1)
                {
                    var position = index + 1;
                    var s = Text.Substring(Location.Position, position - Location.Position);
                    var line = Location.Line + s.Count(c => c == '\n');
                    return new ParseResult<string>(s, new Source(Text, new Location(position, line)));
                }
            }
            return null;
        }

        // Matches the string s
        public ParseResult<string>? Match(string s, bool ignoreCase)
        {
            if (Location.Position + s.Length <= Text.Length &&
                Text.Substring(Location.Position, s.Length).Equals(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                var position = Location.Position + s.Length;
                var line = Location.Line + s.Count(c => c == '\n');
                return new ParseResult<string>(s, new Source(Text, new Location(position, line)));
            }

            return null;
        }
    }
}
