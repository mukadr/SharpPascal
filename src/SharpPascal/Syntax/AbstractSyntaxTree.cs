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

        public abstract void Visit(Visitor visit);
    }

    public sealed class Unit : AbstractSyntaxTree
    {
        public IReadOnlyList<Declaration> Declarations { get; }
        public Statement Main { get; }

        public Unit(Statement main, IReadOnlyList<Declaration>? declarations)
            : base(null)
        {
            Declarations = declarations ?? new List<Declaration>();
            Main = main;
        }

        public Unit(Statement main, params Declaration[] declarations)
            : base(null)
        {
            Declarations = declarations;
            Main = main;
        }

        public override void Visit(Visitor visitor)
        {
            if (visitor.VisitUnit(this))
            {
                foreach (var decl in Declarations)
                {
                    decl.Visit(visitor);
                }
                Main.Visit(visitor);
            }
        }

        public override bool Equals(object obj)
            => obj is Unit unit &&
               unit.Declarations.SequenceEqual(Declarations) &&
               unit.Main.Equals(Main);

        public override int GetHashCode()
            => Declarations.GetHashCode();
    }

    public abstract class Declaration : AbstractSyntaxTree
    {
        protected Declaration(Location? location = null)
            : base(location)
        { }
    }

    public sealed class VarDeclaration : Declaration
    {
        public string Name { get; }
        public string Type { get; }

        public VarDeclaration(string name, string type, Location? location = null)
            : base(location)
        {
            Name = name;
            Type = type;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }

        public override bool Equals(object obj)
            => obj is VarDeclaration @var &&
               @var.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
               @var.Type.Equals(Type, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Type.GetHashCode();
    }

    public abstract class Statement : AbstractSyntaxTree
    {
        protected Statement(Location? location = null)
            : base(location)
        { }
    }

    public class CompoundStatement : Statement
    {
        public IReadOnlyList<Statement> Statements { get; }

        public CompoundStatement(IReadOnlyList<Statement> statements, Location? location = null)
            : base(location)
        {
            Statements = statements;
        }

        public CompoundStatement(params Statement[] statements)
            : base(null)
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
            => obj is CompoundStatement compound &&
               compound.Statements.SequenceEqual(Statements);

        public override int GetHashCode()
            => Statements.GetHashCode();
    }

    public sealed class IfStatement : Statement
    {
        public Expression Expression { get; }
        public Statement TrueStatement { get; }
        public Statement? FalseStatement { get; }

        public IfStatement(Expression expression, Statement trueStatement, Statement? falseStatement = null, Location? location = null)
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
            => obj is IfStatement @if &&
               @if.Expression.Equals(Expression) &&
               @if.TrueStatement.Equals(TrueStatement) &&
               (@if.FalseStatement is null && FalseStatement is null ||
                (@if.FalseStatement != null && @if.FalseStatement.Equals(FalseStatement)));

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class WhileStatement : Statement
    {
        public Expression Expression { get; }
        public Statement Statement { get; }

        public WhileStatement(Expression expression, Statement statement, Location? location = null)
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
            => obj is WhileStatement @while &&
               @while.Expression.Equals(Expression) &&
               @while.Statement.Equals(Statement);

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class AssignmentStatement : Statement
    {
        public string Name { get; }
        public Expression Expression { get; }

        public AssignmentStatement(string name, Expression expression, Location? location = null)
            : base(location)
        {
            Name = name;
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
            => obj is AssignmentStatement assign &&
               assign.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
               assign.Expression.Equals(Expression);

        public override int GetHashCode()
            => Expression.GetHashCode();
    }

    public sealed class ProcedureStatement : Statement
    {
        public CallExpression CallExpression { get; }

        public ProcedureStatement(CallExpression callExpression)
            : base(callExpression.Location)
        {
            CallExpression = callExpression;
        }

        public override void Visit(Visitor visitor)
        {
            CallExpression.Visit(visitor);
        }

        public override bool Equals(object obj)
            => obj is ProcedureStatement proc &&
               proc.CallExpression.Equals(CallExpression);

        public override int GetHashCode()
            => CallExpression.GetHashCode();
    }

    public abstract class Expression : AbstractSyntaxTree
    {
        protected Expression(Location? location = null)
            : base(location)
        { }
    }

    public sealed class IntegerExpression : Expression
    {
        public int Value { get; }

        public IntegerExpression(int value, Location? location = null)
            : base(location)
        {
            Value = value;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitIntegerExpression(this);
        }

        public override bool Equals(object obj)
            => obj is IntegerExpression @int &&
               @int.Value == Value;

        public override int GetHashCode()
            => Value;
    }

    public sealed class StringExpression : Expression
    {
        public string Value { get; }

        public StringExpression(string value, Location? location = null)
            : base(location)
        {
            Value = value;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitStringExpression(this);
        }

        public override bool Equals(object obj)
            => obj is StringExpression str &&
               str.Value == Value;

        public override int GetHashCode()
            => Value.GetHashCode();
    }

    public sealed class VarExpression : Expression
    {
        public string Name { get; }

        public VarExpression(string name, Location? location = null)
            : base(location)
        {
            Name = name;
        }

        public override void Visit(Visitor visitor)
        {
            visitor.VisitVarExpression(this);
        }

        public override bool Equals(object obj)
            => obj is VarExpression @var &&
               @var.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => Name.GetHashCode();
    }

    public abstract class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public string Operator { get; }
        public Expression Right { get; }

        protected BinaryExpression(Expression left, string @operator, Expression right, Location? location = null)
            : base(location)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public static BinaryExpression CreateInstance(Expression left, string @operator, Expression right, Location? location = null)
        {
            return (@operator.ToLower()) switch
            {
                "+" => new AddExpression(left, right, location),
                "-" => new SubExpression(left, right, location),
                "*" => new MulExpression(left, right, location),
                "div" => new DivExpression(left, right, location),
                "mod" => new ModExpression(left, right, location),
                "=" => new EqualExpression(left, right, location),
                "<>" => new NotEqualExpression(left, right, location),
                "<" => new LessThanExpression(left, right, location),
                ">" => new GreaterThanExpression(left, right, location),
                "<=" => new LessOrEqualExpression(left, right, location),
                ">=" => new GreaterOrEqualExpression(left, right, location),
                _ => throw new ArgumentException("Bad operator for BinaryExpression", nameof(@operator)),
            };
        }

        public override void Visit(Visitor visitor)
        {
            Left.Visit(visitor);
            Right.Visit(visitor);
        }

        public override bool Equals(object obj)
            => obj is BinaryExpression bin &&
               bin.Left.Equals(Left) &&
               bin.Operator == Operator &&
               bin.Right.Equals(Right);

        public override int GetHashCode()
            => Left.GetHashCode() ^ Operator.GetHashCode() ^ Right.GetHashCode();
    }

    public sealed class AddExpression : BinaryExpression
    {
        public AddExpression(Expression left, Expression right, Location? location = null)
            : base(left, "+", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitAddExpression(this);
        }
    }

    public sealed class SubExpression : BinaryExpression
    {
        public SubExpression(Expression left, Expression right, Location? location = null)
            : base(left, "-", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitSubExpression(this);
        }
    }

    public sealed class MulExpression : BinaryExpression
    {
        public MulExpression(Expression left, Expression right, Location? location = null)
            : base(left, "*", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitMulExpression(this);
        }
    }

    public sealed class DivExpression : BinaryExpression
    {
        public DivExpression(Expression left, Expression right, Location? location = null)
            : base(left, "div", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitDivExpression(this);
        }
    }

    public sealed class ModExpression : BinaryExpression
    {
        public ModExpression(Expression left, Expression right, Location? location = null)
            : base(left, "mod", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitModExpression(this);
        }
    }

    public sealed class EqualExpression : BinaryExpression
    {
        public EqualExpression(Expression left, Expression right, Location? location = null)
            : base(left, "=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitEqualExpression(this);
        }
    }

    public sealed class NotEqualExpression : BinaryExpression
    {
        public NotEqualExpression(Expression left, Expression right, Location? location = null)
            : base(left, "<>", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitNotEqualExpression(this);
        }
    }

    public sealed class LessThanExpression : BinaryExpression
    {
        public LessThanExpression(Expression left, Expression right, Location? location = null)
            : base(left, "<", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitLessThanExpression(this);
        }
    }

    public sealed class GreaterThanExpression : BinaryExpression
    {
        public GreaterThanExpression(Expression left, Expression right, Location? location = null)
            : base(left, ">", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitGreaterThanExpression(this);
        }
    }

    public sealed class LessOrEqualExpression : BinaryExpression
    {
        public LessOrEqualExpression(Expression left, Expression right, Location? location = null)
            : base(left, "<=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitLessOrEqualExpression(this);
        }
    }

    public sealed class GreaterOrEqualExpression : BinaryExpression
    {
        public GreaterOrEqualExpression(Expression left, Expression right, Location? location = null)
            : base(left, ">=", right, location)
        { }

        public override void Visit(Visitor visitor)
        {
            base.Visit(visitor);
            visitor.VisitGreaterOrEqualExpression(this);
        }
    }

    public sealed class CallExpression : Expression
    {
        public string Name { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        public CallExpression(string name, IReadOnlyList<Expression>? arguments = null, Location? location = null)
            : base(location)
        {
            Name = name;
            Arguments = arguments ?? new List<Expression>();
        }

        public CallExpression(string name, params Expression[] arguments)
            : base(null)
        {
            Name = name;
            Arguments = arguments;
        }

        public override void Visit(Visitor visitor)
        {
            if (visitor.VisitCallExpression(this))
            {
                foreach (var arg in Arguments)
                {
                    arg.Visit(visitor);
                }
            }
        }

        public override bool Equals(object obj)
            => obj is CallExpression call &&
               call.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
               call.Arguments.SequenceEqual(Arguments);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Arguments.GetHashCode();
    }
}
