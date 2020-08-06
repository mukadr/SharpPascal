using System.Collections.Generic;
using System.Linq;

namespace SharpPascal.Syntax
{
    public abstract class AbstractSyntaxTree
    {
        public Location Location { get; }

        public AbstractSyntaxTree(Location location)
        {
            Location = location;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }
    }

    public abstract class Expression : AbstractSyntaxTree
    {
        public Expression(Location location)
            : base(location)
        { }
    }

    public sealed class IntegerExpression : Expression
    {
        public int Value { get; }

        public IntegerExpression(int value, Location location)
            : base(location)
        {
            Value = value;
        }

        public override int GetHashCode()
            => base.GetHashCode() ^ Value;

        public override bool Equals(object obj)
            => obj is IntegerExpression @int &&
               @int.Value == Value;
    }

    public abstract class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public string Operator { get; }
        public Expression Right { get; }

        public BinaryExpression(Expression left, string @operator, Expression right, Location location)
            : base(location)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override int GetHashCode()
            => base.GetHashCode() ^ Left.GetHashCode() ^ Operator.GetHashCode() ^ Right.GetHashCode();

        public override bool Equals(object obj)
            => obj is BinaryExpression bin &&
               bin.Left.Equals(Left) &&
               bin.Operator == Operator &&
               bin.Right.Equals(Right);
    }

    public sealed class AddExpression : BinaryExpression
    {
        public AddExpression(Expression left, Expression right, Location location)
            : base(left, "+", right, location)
        { }
    }

    public sealed class SubExpression : BinaryExpression
    {
        public SubExpression(Expression left, Expression right, Location location)
            : base(left, "-", right, location)
        { }
    }

    public sealed class CallExpression : Expression
    {
        public string Name { get; }
        public IReadOnlyCollection<Expression> Expressions { get; }

        public CallExpression(string name, IReadOnlyCollection<Expression> expressions, Location location)
            : base(location)
        {
            Name = name;
            Expressions = expressions;
        }

        public override int GetHashCode()
            => base.GetHashCode() ^ Name.GetHashCode() ^ Expressions.GetHashCode();

        public override bool Equals(object obj)
            => obj is CallExpression call &&
               call.Name == Name &&
               call.Expressions.SequenceEqual(Expressions);
    }
}
