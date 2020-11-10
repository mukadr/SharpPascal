using SharpPascal.Syntax.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPascal.Syntax
{
    public abstract class AbstractSyntaxTree
    {
        public Location? Location { get; }

        protected AbstractSyntaxTree(Location? location = null)
        {
            Location = location;
        }

        public abstract void Visit(Visitor visitor);
    }

    public sealed class UnitSyntax : AbstractSyntaxTree
    {
        public IReadOnlyList<DeclarationSyntax> Declarations { get; }
        public StatementSyntax Main { get; }

        public UnitSyntax(StatementSyntax main, IReadOnlyList<DeclarationSyntax>? declarations)
        {
            Declarations = declarations ?? new List<DeclarationSyntax>();
            Main = main;
        }

        public UnitSyntax(StatementSyntax main, params DeclarationSyntax[] declarations)
        {
            Declarations = declarations;
            Main = main;
        }

        public override void Visit(Visitor visitor)
        {
            if (!visitor.VisitUnit(this))
            {
                return;
            }

            foreach (var decl in Declarations)
            {
                decl.Visit(visitor);
            }

            Main.Visit(visitor);
        }

        public override bool Equals(object obj)
            => obj is UnitSyntax unit &&
               unit.Declarations.SequenceEqual(Declarations) &&
               unit.Main.Equals(Main);

        public override int GetHashCode()
            => Declarations.GetHashCode();
    }

    public abstract class DeclarationSyntax : AbstractSyntaxTree
    {
        protected DeclarationSyntax(Location? location = null)
            : base(location)
        { }
    }

    public sealed class VarDeclarationSyntax : DeclarationSyntax
    {
        public PascalName Name { get; }
        public PascalName Type { get; }

        public VarDeclarationSyntax(string name, string type, Location? location = null)
            : base(location)
        {
            Name = new PascalName(name);
            Type = new PascalName(type);
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }

        public override bool Equals(object obj)
            => obj is VarDeclarationSyntax @var &&
               @var.Name.Equals(Name) &&
               @var.Type.Equals(Type);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Type.GetHashCode();
    }

    public abstract class StatementSyntax : AbstractSyntaxTree
    {
        protected StatementSyntax(Location? location = null)
            : base(location)
        { }
    }

    public class CompoundStatementSyntax : StatementSyntax
    {
        public IReadOnlyList<StatementSyntax> Statements { get; }

        public CompoundStatementSyntax(IReadOnlyList<StatementSyntax> statements, Location? location = null)
            : base(location)
        {
            Statements = statements;
        }

        public CompoundStatementSyntax(params StatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override void Visit(Visitor visitor)
        {
            foreach (var stmt in Statements)
            {
                stmt.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is CompoundStatementSyntax compound &&
               compound.Statements.SequenceEqual(Statements);

        public override int GetHashCode()
            => Statements.GetHashCode();
    }

    public sealed class IfStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression { get; }
        public StatementSyntax TrueStatement { get; }
        public StatementSyntax? FalseStatement { get; }

        public IfStatementSyntax(ExpressionSyntax expression, StatementSyntax trueStatement, StatementSyntax? falseStatement = null, Location? location = null)
            : base(location)
        {
            Expression = expression;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
        }

        public override void Visit(Visitor visitor)
        {
            if (visitor.VisitIfStatement(this))
            {
                Expression.Visit(visitor);
                TrueStatement.Visit(visitor);
                FalseStatement?.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is IfStatementSyntax @if &&
               @if.Expression.Equals(Expression) &&
               @if.TrueStatement.Equals(TrueStatement) &&
               (@if.FalseStatement is null && FalseStatement is null ||
                (@if.FalseStatement != null && @if.FalseStatement.Equals(FalseStatement)));

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression { get; }
        public StatementSyntax Statement { get; }

        public WhileStatementSyntax(ExpressionSyntax expression, StatementSyntax statement, Location? location = null)
            : base(location)
        {
            Expression = expression;
            Statement = statement;
        }

        public override void Visit(Visitor visitor)
        {
            if (visitor.VisitWhileStatement(this))
            {
                Expression.Visit(visitor);
                Statement.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is WhileStatementSyntax @while &&
               @while.Expression.Equals(Expression) &&
               @while.Statement.Equals(Statement);

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class AssignmentStatementSyntax : StatementSyntax
    {
        public PascalName Name { get; }
        public ExpressionSyntax Expression { get; }

        public AssignmentStatementSyntax(string name, ExpressionSyntax expression, Location? location = null)
            : base(location)
        {
            Name = new PascalName(name);
            Expression = expression;
        }

        public override void Visit(Visitor visitor)
        {
            if (visitor.VisitAssignmentStatement(this))
            {
                Expression.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is AssignmentStatementSyntax assign &&
               assign.Name.Equals(Name) &&
               assign.Expression.Equals(Expression);

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class ProcedureStatementSyntax : StatementSyntax
    {
        public CallExpressionSyntax CallExpression { get; }

        public ProcedureStatementSyntax(CallExpressionSyntax callExpression)
            : base(callExpression.Location)
        {
            CallExpression = callExpression;
        }

        public override void Visit(Visitor visitor)
        {
            CallExpression.Visit(visitor);
        }

        public override bool Equals(object obj)
            => obj is ProcedureStatementSyntax proc &&
               proc.CallExpression.Equals(CallExpression);

        public override int GetHashCode()
            => CallExpression.GetHashCode();
    }

    public abstract class ExpressionSyntax : AbstractSyntaxTree
    {
        protected ExpressionSyntax(Location? location = null)
            : base(location)
        { }
    }

    public sealed class IntegerExpressionSyntax : ExpressionSyntax
    {
        public int Value { get; }

        public IntegerExpressionSyntax(int value, Location? location = null)
            : base(location)
        {
            Value = value;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitIntegerExpression(this);
        }

        public override bool Equals(object obj)
            => obj is IntegerExpressionSyntax @int &&
               @int.Value == Value;

        public override int GetHashCode()
            => Value;
    }

    public sealed class StringExpressionSyntax : ExpressionSyntax
    {
        public string Value { get; }

        public StringExpressionSyntax(string value, Location? location = null)
            : base(location)
        {
            Value = value;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitStringExpression(this);
        }

        public override bool Equals(object obj)
            => obj is StringExpressionSyntax str &&
               str.Value == Value;

        public override int GetHashCode()
            => Value.GetHashCode();
    }

    public sealed class VarExpressionSyntax : ExpressionSyntax
    {
        public PascalName Name { get; }

        public VarExpressionSyntax(string name, Location? location = null)
            : base(location)
        {
            Name = new PascalName(name);
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitVarExpression(this);
        }

        public override bool Equals(object obj)
            => obj is VarExpressionSyntax @var &&
               @var.Name.Equals(Name);

        public override int GetHashCode()
            => Name.GetHashCode();
    }

    public abstract class BinaryExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; }
        public string Operator { get; }
        public ExpressionSyntax Right { get; }

        protected BinaryExpressionSyntax(ExpressionSyntax left, string @operator, ExpressionSyntax right, Location? location = null)
            : base(location)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public static BinaryExpressionSyntax CreateInstance(ExpressionSyntax left, string @operator, ExpressionSyntax right, Location? location = null)
        {
            return (@operator.ToLower()) switch
            {
                "+" => new AddExpressionSyntax(left, right, location),
                "-" => new SubExpressionSyntax(left, right, location),
                "*" => new MulExpressionSyntax(left, right, location),
                "div" => new DivExpressionSyntax(left, right, location),
                "mod" => new ModExpressionSyntax(left, right, location),
                "=" => new EqualExpressionSyntax(left, right, location),
                "<>" => new NotEqualExpressionSyntax(left, right, location),
                "<" => new LessThanExpressionSyntax(left, right, location),
                ">" => new GreaterThanExpressionSyntax(left, right, location),
                "<=" => new LessOrEqualExpressionSyntax(left, right, location),
                ">=" => new GreaterOrEqualExpressionSyntax(left, right, location),
                _ => throw new ArgumentException("Bad operator for BinaryExpression", nameof(@operator)),
            };
        }

        public override void Visit(Visitor visitor)
        {
            Left.Visit(visitor);
            Right.Visit(visitor);
        }

        public override bool Equals(object obj)
            => obj is BinaryExpressionSyntax bin &&
               bin.Left.Equals(Left) &&
               bin.Operator == Operator &&
               bin.Right.Equals(Right);

        public override int GetHashCode()
            => Left.GetHashCode() ^ Operator.GetHashCode() ^ Right.GetHashCode();
    }

    public sealed class AddExpressionSyntax : BinaryExpressionSyntax
    {
        public AddExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "+", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitAddExpression(this);
        }
    }

    public sealed class SubExpressionSyntax : BinaryExpressionSyntax
    {
        public SubExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "-", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitSubExpression(this);
        }
    }

    public sealed class MulExpressionSyntax : BinaryExpressionSyntax
    {
        public MulExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "*", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitMulExpression(this);
        }
    }

    public sealed class DivExpressionSyntax : BinaryExpressionSyntax
    {
        public DivExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "div", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitDivExpression(this);
        }
    }

    public sealed class ModExpressionSyntax : BinaryExpressionSyntax
    {
        public ModExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "mod", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitModExpression(this);
        }
    }

    public sealed class EqualExpressionSyntax : BinaryExpressionSyntax
    {
        public EqualExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitEqualExpression(this);
        }
    }

    public sealed class NotEqualExpressionSyntax : BinaryExpressionSyntax
    {
        public NotEqualExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "<>", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitNotEqualExpression(this);
        }
    }

    public sealed class LessThanExpressionSyntax : BinaryExpressionSyntax
    {
        public LessThanExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "<", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitLessThanExpression(this);
        }
    }

    public sealed class GreaterThanExpressionSyntax : BinaryExpressionSyntax
    {
        public GreaterThanExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, ">", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitGreaterThanExpression(this);
        }
    }

    public sealed class LessOrEqualExpressionSyntax : BinaryExpressionSyntax
    {
        public LessOrEqualExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, "<=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitLessOrEqualExpression(this);
        }
    }

    public sealed class GreaterOrEqualExpressionSyntax : BinaryExpressionSyntax
    {
        public GreaterOrEqualExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Location? location = null)
            : base(left, ">=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitGreaterOrEqualExpression(this);
        }
    }

    public sealed class CallExpressionSyntax : ExpressionSyntax
    {
        public PascalName Name { get; }
        public IReadOnlyList<ExpressionSyntax> Arguments { get; }

        public CallExpressionSyntax(string name, IReadOnlyList<ExpressionSyntax>? arguments = null, Location? location = null)
            : base(location)
        {
            Name = new PascalName(name);
            Arguments = arguments ?? new List<ExpressionSyntax>();
        }

        public CallExpressionSyntax(string name, params ExpressionSyntax[] arguments)
        {
            Name = new PascalName(name);
            Arguments = arguments;
        }

        public override void Visit(Visitor visitor)
        {
            if (!visitor.VisitCallExpression(this))
            {
                return;
            }

            foreach (var arg in Arguments)
            {
                arg.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is CallExpressionSyntax call &&
               call.Name.Equals(Name) &&
               call.Arguments.SequenceEqual(Arguments);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Arguments.GetHashCode();
    }
}
