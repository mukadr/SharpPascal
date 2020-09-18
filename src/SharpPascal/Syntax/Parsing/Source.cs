using System.Linq;
using System.Text.RegularExpressions;

namespace SharpPascal.Syntax.Parsing
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

        // Matches a regular expression
        public ParseResult<string>? Match(string pattern, bool ignoreCase)
        {
            var options = RegexOptions.Multiline | RegexOptions.Compiled;
            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }
            var regex = new Regex("\\G" + pattern, options);
            var match = regex.Match(Text, Location.Position);
            if (match.Success)
            {
                var position = Location.Position + match.Length;
                var line = Location.Line + match.Value.Count(c => c == '\n');
                return new ParseResult<string>(match.Value, new Source(Text, new Location(position, line)));
            }
            return null;
        }
    }
}
