using SharpPascal.Semantic;
using SharpPascal.Syntax;

namespace SharpPascal.Binding
{
    public sealed class BoundUnit
    {
        public UnitSyntax UnitSyntax { get; }

        public Scope Scope { get; }

        public BoundUnit(UnitSyntax unit, Scope scope)
        {
            UnitSyntax = unit;
            Scope = scope;
        }

        public override bool Equals(object obj)
            => obj is BoundUnit unit &&
               unit.UnitSyntax.Equals(UnitSyntax) &&
               unit.Scope.Equals(Scope);

        public override int GetHashCode()
            => UnitSyntax.GetHashCode() ^ Scope.GetHashCode();
    }
}
