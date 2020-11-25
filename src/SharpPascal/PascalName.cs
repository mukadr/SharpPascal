using System;

namespace SharpPascal
{
    public class PascalName
    {
        public string Name { get; }

        public PascalName(string name)
        {
            Name = name;
        }

        public override bool Equals(object other)
            => other is PascalName pn &&
               pn.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => Name.GetHashCode();

        public static implicit operator PascalName(string str) => new PascalName(str);
        public static bool operator==(PascalName left, PascalName right) => left.Equals(right);
        public static bool operator!=(PascalName left, PascalName right) => !(left == right);
    }
}
