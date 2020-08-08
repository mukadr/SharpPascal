using SharpPascal.Syntax.Parsing;
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
                Symbol('{')
                .And(Until('}').OrError("expected '}' before end of source"));

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            var letter =
                Symbol('_')
                .Or(Symbol('a', 'z'))
                .Or(Symbol('A', 'Z'));

            var digit =
                Symbol('0', '9');

            Parser<string> parseKeyword(string text)
                => Text(text)
                   .And(Not(letter.Or(digit)))
                   .And(Constant(text))
                   .Skip(blank);

            var add =
                Text("+")
                .Skip(blank);

            var sub =
                Text("-")
                .Skip(blank);

            var mul =
                Text("*")
                .Skip(blank);

            var div =
                parseKeyword("div");

            var keyword =
                div;

            var id =
                Not(keyword)
                .And(letter.Bind(l =>
                    ZeroOrMore(letter.Or(digit)).Map(ld =>
                        l + ld)));

            var lparen =
                Text("(")
                .Skip(blank);

            var rparen =
                Text(")")
                .Skip(blank);

            var integer =
                OneOrMore(digit).Map<Expression>((value, line) =>
                    new IntegerExpression(int.Parse(value), new Location(line)))
                .Skip(blank);

            var variable =
                id.Map<Expression>((name, line) =>
                    (Expression)new VarExpression(name, new Location(line)))
                .Skip(blank);

            var expression =
                Forward<Expression>();

            var factor =
                integer
                .Or(variable)
                .Or(lparen.And(expression).Bind(e => rparen.And(Constant(e))));

            var mulExpression =
                factor.Bind(first =>
                    ZeroOrMore(mul.Or(div).Bind((op, line) =>
                        factor.Map(right =>
                            (op, right, line)))).Map(operatorTerms =>
                                operatorTerms.Aggregate(first, (left, ot) =>
                                    BinaryExpression.CreateInstance(left, ot.op, ot.right, new Location(ot.line)))));

            var addExpression =
                mulExpression.Bind(first =>
                    ZeroOrMore(add.Or(sub).Bind((op, line) =>
                        mulExpression.Map(right =>
                            (op, right, line)))).Map(operatorTerms =>
                                operatorTerms.Aggregate(first, (left, ot) =>
                                    BinaryExpression.CreateInstance(left, ot.op, ot.right, new Location(ot.line)))));

            expression.Parse =
                addExpression.Parse;

            var program =
                Maybe(blank).And(Maybe(expression));

            return program.ParseToCompletion(text);
        }
    }
}
