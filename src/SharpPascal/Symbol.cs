namespace SharpPascal
{
    public class Symbol
    {
        public PascalName Name { get; }
        public Type Type { get; }

        public Symbol(PascalName name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override bool Equals(object other)
            => other is Symbol symbol &&
               symbol.Name.Equals(Name) &&
               symbol.Type.Equals(Type);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Type.GetHashCode();
    }
}
