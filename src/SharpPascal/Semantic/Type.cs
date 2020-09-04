using SharpPascal.Syntax;

namespace SharpPascal.Semantic
{
    public class Type
    {
        public PascalName Name { get; }

        public Type(PascalName name)
        {
            Name = name;
        }

        public override bool Equals(object other)
            => other is Type type &&
               type.Name == Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}
