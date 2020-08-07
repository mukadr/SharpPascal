using SharpPascal.Syntax.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using static SharpPascal.Syntax.Parsing.ParserFactory;

namespace SharpPascal.Syntax
{
    public static class PascalParser
    {
        public static (AbstractSyntaxTree? Tree, int Line) Parse(Source source)
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

            var skipWhite =
                OneOrMore(whitespace.Or(multilineComment));

            var letter =
                Symbol('_')
                .Or(Symbol('a', 'z'))
                .Or(Symbol('A', 'Z'));

            var digit =
                Symbol('0', '9');

            Func<string, Parser<string>> parseKeyword = (text)
                => Text(text)
                   .And(Not(letter.Or(digit)))
                   .And(Constant(text))
                   .Consume(skipWhite);

            var add =
                Text("+")
                .Consume(whitespace);

            var sub =
                Text("-")
                .Consume(whitespace);

            var mul =
                Text("*")
                .Consume(whitespace);

            var div =
                parseKeyword("div");

            var integer =
                OneOrMore(digit)
                .Map(value => new IntegerExpression(int.Parse(value), new Location(currentLine)))
                .Consume(whitespace);

            var factor =
                integer;

            var mulExpression =
                factor.Bind(first =>
                    ZeroOrMore(mul.Or(div).Bind(op => factor.Bind(right => Constant((op, right))))).Bind(terms =>
                    {
                        Expression left = first;

                        foreach (var term in terms)
                        {
                            left = new BinaryExpression(left, term.op, term.right, new Location(currentLine));
                        }

                        return Constant(left);
                    })
                );

            var addExpression =
                mulExpression.Bind(first =>
                    ZeroOrMore(add.Or(sub).Bind(op => mulExpression.Bind(right => Constant((op, right))))).Bind(terms =>
                    {
                        Expression left = first;

                        foreach (var term in terms)
                        {
                            left = new BinaryExpression(left, term.op, term.right, new Location(currentLine));
                        }

                        return Constant(left);
                    })
                );

            var expression =
                addExpression;

            var program =
                Maybe(skipWhite).And(expression);

            return (program.Parse(source)?.Value, currentLine);
        }
    }
}
