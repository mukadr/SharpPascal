namespace SharpPascal.Syntax.Parsing
{
    public class Location
    {
        public int Position { get; }
        public int Line { get; }

        public Location(int position, int line)
        {
            Position = position;
            Line = line;
        }
    }
}
