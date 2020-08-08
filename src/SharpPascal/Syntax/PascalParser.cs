using SharpPascal.Syntax.Parsing;
using System;
using System.Linq;
using static SharpPascal.Syntax.Parsing.ParserFactory;

namespace SharpPascal.Syntax
{
    public static class PascalParser
    {
        public static AbstractSyntaxTree? Parse(string source)
        {
            var currentLine = 1;

            Func<string, string> report = (error) => string.Format("%d: %s", currentLine, error);

            var whitespace =
                OneOrMore(
                    Symbol(' ')
                    .Or(Symbol('\t'))
                    .Or(Symbol('\n').Bind(c =>
                    {
                        currentLine++;
                        return Constant(c);
                    }))
                    .Or(Symbol('\r'))
                );

            var multilineComment =
                Symbol('{')
                .And(Until('}').OrError(report("Expected '}' before end of source")))
                .Bind(text =>
                {
                    currentLine += text.Count(c => c == '\n');
                    return Constant(text);
                });

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            var letter =
                Symbol('_')
                .Or(Symbol('a', 'z'))
                .Or(Symbol('A', 'Z'));

            var digit =
                Symbol('0', '9');

            Func<string, Parser<string>> parseKeyword = (text) =>
                Text(text)
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
                    ZeroOrMore(letter.Or(digit)).Bind(ld =>
                        Constant(l + ld))));

            var lparen =
                Text("(")
                .Skip(blank);

            var rparen =
                Text(")")
                .Skip(blank);

            var integer =
                OneOrMore(digit)
                .Map<Expression>(value => new IntegerExpression(int.Parse(value), new Location(currentLine)))
                .Skip(blank);

            var variable =
                id
                .Map<Expression>(name => new VarExpression(name, new Location(currentLine)))
                .Skip(blank);

            var expression =
                Forward<Expression>();

            var factor =
                integer
                .Or(variable)
                .Or(lparen.And(expression).Bind(e => rparen.And(Constant(e))));

            var mulExpression =
                factor.Bind(first =>
                    ZeroOrMore(mul.Or(div).Bind(op => factor.Bind(right => Constant((op, right))))).Bind(operatorTerms =>
                        Constant(operatorTerms.Aggregate(first, (left, ot) =>
                            BinaryExpression.CreateInstance(left, ot.op, ot.right, new Location(currentLine))))));

            var addExpression =
                mulExpression.Bind(first =>
                    ZeroOrMore(add.Or(sub).Bind(op => mulExpression.Bind(right => Constant((op, right))))).Bind(operatorTerms =>
                        Constant(operatorTerms.Aggregate(first, (left, ot) =>
                            BinaryExpression.CreateInstance(left, ot.op, ot.right, new Location(currentLine))))));

            expression.Parse =
                addExpression.Parse;

            var program =
                Maybe(blank).And(Maybe(expression));

            return program.ParseString(source);
        }
    }
}
