namespace SharpPascal
{
    public class Type
    {
        public static readonly Type Unknown = new Type("unknown");
        public static readonly Type Boolean = new Type("boolean");
        public static readonly Type Integer = new Type("integer");

        public PascalName Name { get; }

        public Type(PascalName name)
        {
            Name = name;
        }

        public override bool Equals(object? other)
            => other is Type type &&
               type.Name == Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}
