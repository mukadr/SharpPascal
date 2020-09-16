using SharpPascal.Semantic;
using SharpPascal.Syntax;

namespace SharpPascal.Binding
{
    public static class Binder
    {
        public static BoundUnit SyntaxCheck(UnitSyntax unit)
        {
            var unitScope = new Scope();
            unitScope.Add(new Symbol("integer", new Type("integer")));

            var errors = 0;
            var currentScope = unitScope;

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    var typeSymbol = currentScope.Find(var.Type);
                    if (typeSymbol == null)
                    {
                        // TODO: Error reporting
                        errors++;
                        return;
                    }
                    currentScope.Add(new Symbol(var.Name, typeSymbol.Type));
                }
            });

            return new BoundUnit(unit, unitScope);
        }
    }
}
