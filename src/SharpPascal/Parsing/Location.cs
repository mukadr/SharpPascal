namespace SharpPascal.Parsing
{
    public class Location
    {
        public static readonly Location Unknown = new Location(0, 0);

        public int Position { get; }
        public int Line { get; }

        public Location(int position, int line)
        {
            Position = position;
            Line = line;
        }
    }
}
