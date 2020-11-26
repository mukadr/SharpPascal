namespace SharpPascal
{
    public static class Analyzer
    {
        private static void RegisterPascalTypes(Scope scope)
        {
            scope.Add(new Symbol("integer", Type.Integer));
        }

        public static DiagnosticList TypeCheck(Unit unit)
        {
            var diagnostics = new DiagnosticList();

            RegisterPascalTypes(unit.Scope);

            var currentScope = new Scope(unit.Scope);

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    var symbol = currentScope.Find(var.Name);
                    if (symbol != null)
                    {
                        diagnostics.AddError($"Variable `{var.Name}` redeclared.", var.Location);
                        return;
                    }

                    var type = currentScope.Find(var.TypeName);
                    if (type == null)
                    {
                        diagnostics.AddError($"Unknown type `{var.TypeName}`.", var.Location);
                        return;
                    }

                    var.Type = type.Type;

                    currentScope.Add(new Symbol(var.Name, type.Type));
                }
            });

            return diagnostics;
        }
    }
}
