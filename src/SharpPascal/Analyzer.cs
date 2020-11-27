namespace SharpPascal
{
    public static class Analyzer
    {
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
                },

                VisitIntegerExpression = integer =>
                {
                    integer.Type = Type.Integer;
                },

                VisitVarExpression = var =>
                {
                    var symbol = currentScope.Find(var.Name);
                    if (symbol == null)
                    {
                        diagnostics.AddError($"Undeclared symbol `{var.Name}`.", var.Location);
                        return;
                    }

                    var.Type = symbol.Type;
                },

                VisitAddExpression = add =>
                {
                    add.Type = ResolveArithmeticExpression(add);
                },

                VisitSubExpression = sub =>
                {
                    sub.Type = ResolveArithmeticExpression(sub);
                },

                VisitMulExpression = mul =>
                {
                    mul.Type = ResolveArithmeticExpression(mul);
                },

                VisitDivExpression = div =>
                {
                    div.Type = ResolveArithmeticExpression(div);
                },

                VisitModExpression = mod =>
                {
                    mod.Type = ResolveArithmeticExpression(mod);
                },

                VisitEqualExpression = eq =>
                {
                    eq.Type = ResolveEqualityExpression(eq);
                },

                VisitNotEqualExpression = ne =>
                {
                    ne.Type = ResolveEqualityExpression(ne);
                },

                VisitLessThanExpression = lt =>
                {
                    lt.Type = ResolveRelationalExpression(lt);
                },

                VisitGreaterThanExpression = gt =>
                {
                    gt.Type = ResolveRelationalExpression(gt);
                },

                VisitLessOrEqualExpression = le =>
                {
                    le.Type = ResolveRelationalExpression(le);
                },

                VisitGreaterOrEqualExpression = ge =>
                {
                    ge.Type = ResolveRelationalExpression(ge);
                }
            });

            Type ResolveArithmeticExpression(BinaryExpression bin)
            {
                if (bin.Left.Type == Type.Unknown || bin.Right.Type == Type.Unknown)
                {
                    return Type.Unknown;
                }

                if (bin.Left.Type != Type.Integer || bin.Right.Type != Type.Integer)
                {
                    diagnostics.AddError("Incompatible types in binary expression.", bin.Location);
                }

                return Type.Integer;
            }

            Type ResolveEqualityExpression(BinaryExpression bin)
            {
                if (bin.Left.Type == Type.Unknown || bin.Right.Type == Type.Unknown)
                {
                    return Type.Unknown;
                }

                if (bin.Left.Type != bin.Right.Type)
                {
                    diagnostics.AddError("Incompatible types in binary expression.", bin.Location);
                }

                return Type.Boolean;
            }

            Type ResolveRelationalExpression(BinaryExpression bin)
            {
                if (bin.Left.Type == Type.Unknown || bin.Right.Type == Type.Unknown)
                {
                    return Type.Unknown;
                }

                if (bin.Left.Type != Type.Integer || bin.Right.Type != Type.Integer)
                {
                    diagnostics.AddError("Incompatible types in binary expression.", bin.Location);
                }

                return Type.Boolean;
            }

            return diagnostics;
        }

        private static void RegisterPascalTypes(Scope scope)
        {
            scope.Add(new Symbol("boolean", Type.Boolean));
            scope.Add(new Symbol("integer", Type.Integer));
        }
    }
}
