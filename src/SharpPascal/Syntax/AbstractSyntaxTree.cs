using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPascal.Syntax
{
    public abstract class AbstractSyntaxTree
    {
        public Location? Location { get; }

        protected AbstractSyntaxTree(Location? location = default)
        {
            Location = location;
        }
    }

    public abstract class Expression : AbstractSyntaxTree
    {
        protected Expression(Location? location = default)
            : base(location)
        { }
    }

    public sealed class IntegerExpression : Expression
    {
        public int Value { get; }

        public IntegerExpression(int value, Location? location = default)
            : base(location)
        {
            Value = value;
        }

        public override int GetHashCode()
            => Value;

        public override bool Equals(object obj)
            => obj is IntegerExpression @int &&
               @int.Value == Value;
    }

    public sealed class VarExpression : Expression
    {
        public string Name { get; }

        public VarExpression(string name, Location? location = default)
            : base(location)
        {
            Name = name;
        }

        public override int GetHashCode()
            => Name.GetHashCode();

        public override bool Equals(object obj)
            => obj is VarExpression @var &&
               @var.Name == Name;
    }

    public abstract class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public string Operator { get; }
        public Expression Right { get; }

        protected BinaryExpression(Expression left, string @operator, Expression right, Location? location = default)
            : base(location)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public static BinaryExpression CreateInstance(Expression left, string @operator, Expression right, Location? location = default)
        {
            switch (@operator)
            {
                case "+": return new AddExpression(left, right, location);
                case "-": return new SubExpression(left, right, location);
                case "*": return new MulExpression(left, right, location);
                case "div": return new DivExpression(left, right, location);
                case "=": return new EqualExpression(left, right, location);
                case "<>": return new NotEqualExpression(left, right, location);
                case "<": return new LessThanExpression(left, right, location);
                case ">": return new GreaterThanExpression(left, right, location);
                case "<=": return new LessOrEqualExpression(left, right, location);
                case ">=": return new GreaterOrEqualExpression(left, right, location);
                default: throw new ArgumentException("Bad operator for BinaryExpression", nameof(@operator));
            }
        }

        public override int GetHashCode()
            => Left.GetHashCode() ^ Operator.GetHashCode() ^ Right.GetHashCode();

        public override bool Equals(object obj)
            => obj is BinaryExpression bin &&
               bin.Left.Equals(Left) &&
               bin.Operator == Operator &&
               bin.Right.Equals(Right);
    }

    public sealed class AddExpression : BinaryExpression
    {
        public AddExpression(Expression left, Expression right, Location? location = default)
            : base(left, "+", right, location)
        { }
    }

    public sealed class SubExpression : BinaryExpression
    {
        public SubExpression(Expression left, Expression right, Location? location = default)
            : base(left, "-", right, location)
        { }
    }

    public sealed class MulExpression : BinaryExpression
    {
        public MulExpression(Expression left, Expression right, Location? location = default)
            : base(left, "*", right, location)
        { }
    }

    public sealed class DivExpression : BinaryExpression
    {
        public DivExpression(Expression left, Expression right, Location? location = default)
            : base(left, "div", right, location)
        { }
    }

    public sealed class EqualExpression : BinaryExpression
    {
        public EqualExpression(Expression left, Expression right, Location? location = default)
            : base(left, "=", right, location)
        { }
    }

    public sealed class NotEqualExpression : BinaryExpression
    {
        public NotEqualExpression(Expression left, Expression right, Location? location = default)
            : base(left, "<>", right, location)
        { }
    }

    public sealed class LessThanExpression : BinaryExpression
    {
        public LessThanExpression(Expression left, Expression right, Location? location = default)
            : base(left, "<", right, location)
        { }
    }

    public sealed class GreaterThanExpression : BinaryExpression
    {
        public GreaterThanExpression(Expression left, Expression right, Location? location = default)
            : base(left, ">", right, location)
        { }
    }

    public sealed class LessOrEqualExpression : BinaryExpression
    {
        public LessOrEqualExpression(Expression left, Expression right, Location? location = default)
            : base(left, "<=", right, location)
        { }
    }

    public sealed class GreaterOrEqualExpression : BinaryExpression
    {
        public GreaterOrEqualExpression(Expression left, Expression right, Location? location = default)
            : base(left, ">=", right, location)
        { }
    }

    public sealed class CallExpression : Expression
    {
        public string Name { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        public CallExpression(string name, IReadOnlyList<Expression> arguments, Location? location = default)
            : base(location)
        {
            Name = name;
            Arguments = arguments;
        }

        public override int GetHashCode()
            => Name.GetHashCode() ^ Arguments.GetHashCode();

        public override bool Equals(object obj)
            => obj is CallExpression call &&
               call.Name == Name &&
               call.Arguments.SequenceEqual(Arguments);
    }

    public abstract class Statement : AbstractSyntaxTree
    {
        protected Statement(Location? location = default)
            : base(location)
        { }
    }

    public class CompoundStatement : Statement
    {
        public IReadOnlyList<Statement> Statements { get; }

        public CompoundStatement(IReadOnlyList<Statement> statements, Location? location = default)
            : base(location)
        {
            Statements = statements;
        }

        public CompoundStatement(params Statement[] statements)
            : base(null)
        {
            Statements = statements;
        }

        public override int GetHashCode()
            => Statements.GetHashCode();

        public override bool Equals(object obj)
            => obj is CompoundStatement compound &&
               compound.Statements.SequenceEqual(Statements);
    }

    public sealed class IfStatement : Statement
    {
        public Expression Expression { get; }
        public Statement TrueStatement { get; }
        public Statement? FalseStatement { get; }

        public IfStatement(Expression expression, Statement trueStatement, Statement? falseStatement = null, Location? location = default)
            : base(location)
        {
            Expression = expression;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
        }

        public override int GetHashCode()
            => Expression.GetHashCode();

        public override bool Equals(object obj)
            => obj is IfStatement @if &&
               @if.Expression.Equals(Expression) &&
               @if.TrueStatement.Equals(TrueStatement) &&
               (@if.FalseStatement is null && FalseStatement is null ||
                (@if.FalseStatement != null && @if.FalseStatement.Equals(FalseStatement)));
    }

    public sealed class AssignmentStatement : Statement
    {
        public string Name { get; }
        public Expression Expression { get; }

        public AssignmentStatement(string name, Expression expression, Location? location = default)
            : base(location)
        {
            Name = name;
            Expression = expression;
        }

        public override int GetHashCode()
            => Expression.GetHashCode();

        public override bool Equals(object obj)
            => obj is AssignmentStatement assign &&
               assign.Name == Name &&
               assign.Expression.Equals(Expression);
    }
}
