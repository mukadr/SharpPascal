namespace SharpPascal.Syntax
{
    public class Location
    {
        public int Line { get; }

        public Location(int line)
        {
            Line = line;
        }

        public override int GetHashCode()
        {
            return Line;
        }
    }
}
