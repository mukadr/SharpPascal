using SharpPascal.Syntax.Parsing;
using System.Collections.Generic;
using System.Linq;
using static SharpPascal.Syntax.Parsing.ParserFactory;

namespace SharpPascal.Syntax
{
    public static class PascalParser
    {
        public static AbstractSyntaxTree? Parse(string text)
        {
            var whitespace =
                OneOrMore(
                    Symbol(' ')
                    .Or(Symbol('\t'))
                    .Or(Symbol('\n'))
                    .Or(Symbol('\r')));

            var multilineComment =
                Symbol('{').And(Until('}').OrError("expected '}' before end of source"));

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            var letter =
                Symbol('_')
                .Or(Symbol('a', 'z'))
                .Or(Symbol('A', 'Z'));

            var digit =
                Symbol('0', '9');

            Parser<(string text, Location location)> op(string text) =>
                Text(text)
                .Map((_, line) => (text, new Location(line)))
                .Skip(blank);

            var assign = op(":=");
            var add = op("+");
            var sub = op("-");
            var mul = op("*");
            var eq = op("=");
            var ne = op("<>");
            var le = op("<=");
            var ge = op(">=");
            var lt = op("<");
            var gt = op(">");
            var lparen = op("(");
            var rparen = op(")");
            var semi = op(";");
            var comma = op(",");
            var dot = op(".");

            Parser<(string text, Location location)> kw(string text) =>
                Text(text, ignoreCase: true)
                .And(Not(letter.Or(digit)))
                .Map((_, line) => (text, new Location(line)))
                .Skip(blank);

            var begin = kw("begin");
            var div = kw("div");
            var @else = kw("else");
            var end = kw("end");
            var @if = kw("if");
            var then = kw("then");

            var keyword =
                begin
                .Or(div)
                .Or(@else)
                .Or(end)
                .Or(@if)
                .Or(then);

            var id =
                Not(keyword)
                .And(letter.Bind(l =>
                    ZeroOrMore(letter.Or(digit)).Map((ld, line) =>
                        (text: l + ld, location: new Location(line)))))
                .Skip(blank);

            var integer =
                OneOrMore(digit)
                .Map<Expression>((value, line) => new IntegerExpression(int.Parse(value), new Location(line)))
                .Skip(blank);

            var variable =
                id.Map<Expression>(id => new VarExpression(id.text, id.location));

            var expression =
                Forward<Expression>();

            var args =
                expression.Bind(arg =>
                    ZeroOrMore(comma.And(expression)).Map(args =>
                    {
                        args.Insert(0, arg);
                        return args;
                    }))
                .Or(Constant(new List<Expression>()));

            var call =
                id.Bind(id =>
                    lparen.And(args.Bind(args =>
                        rparen.Map<Expression>(_ => new CallExpression(id.text, args, id.location)))));

            var factor =
                integer
                .Or(call)
                .Or(variable)
                .Or(lparen.And(expression).Bind(expr => rparen.And(Constant(expr))));

            Parser<Expression> binaryOperator(Parser<Expression> expr, Parser<(string text, Location location)> op) =>
                expr.Bind(first =>
                    ZeroOrMore(op.Bind(op =>
                        expr.Map(right =>
                            (op, right)))).Map(operatorTerms =>
                                operatorTerms.Aggregate(first, (left, ot) =>
                                    BinaryExpression.CreateInstance(left, ot.op.text, ot.right, ot.op.location))));

            var mulExpression =
                binaryOperator(factor, mul.Or(div));

            var addExpression =
                binaryOperator(mulExpression, add.Or(sub));

            var ltExpression =
                binaryOperator(addExpression, le.Or(ge).Or(lt).Or(gt));

            var eqExpression =
                binaryOperator(ltExpression, eq.Or(ne));

            expression.Parse =
                eqExpression.Parse;

            var statement =
                Forward<Statement>();

            var ifStatement =
                @if.Bind(@if =>
                    expression.Bind(expr =>
                        then.And(statement.Bind(trueStmt =>
                            Maybe(@else.And(statement)).Map<Statement>(falseStmt =>
                                new IfStatement(expr, trueStmt, falseStmt, @if.location))))));

            var assignmentStatement =
                id.Bind(id =>
                    assign.And(expression.Map<Statement>(expr =>
                        new AssignmentStatement(id.text, expr, id.location))));

            statement.Parse =
                ifStatement
                .Or(assignmentStatement)
                .Parse;

            var compoundStmt =
                begin.And(
                    Maybe(statement).Bind(first =>
                        ZeroOrMore(semi.And(statement)).Bind(stmts =>
                        {
                            if (first != null)
                            {
                                stmts.Insert(0, first);
                            }
                            return Constant(new CompoundStatement(stmts, first?.Location));
                        }))
                ).Bind(compound => end.Map(_ => compound));

            var program =
                Maybe(blank).And(compoundStmt.Bind(compound => dot.Map(_ => compound)));

            return program.ParseToCompletion(text);
        }
    }
}
