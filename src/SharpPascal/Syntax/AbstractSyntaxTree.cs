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

    public sealed class CallExpression : Expression
    {
        public string Name { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }

        public CallExpression(string name, IReadOnlyCollection<Expression> arguments, Location? location = default)
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

    public sealed class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression, Location? location = default)
            : base(location)
        {
            Expression = expression;
        }

        public override int GetHashCode()
            => Expression.GetHashCode();

        public override bool Equals(object obj)
            => obj is ExpressionStatement stmt &&
               stmt.Expression.Equals(Expression);
    }
}
