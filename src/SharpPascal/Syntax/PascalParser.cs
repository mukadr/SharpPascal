using SharpPascal.Syntax.Parsing;
using System;
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

            var digit =
                Symbol('0', '9');

            var integer =
                OneOrMore(digit)
                .Map(value => new IntegerExpression(int.Parse(value), new Location(currentLine)));

            var program =
                skipWhite.Anyway(integer);

            return (program.Parse(source)?.Value, currentLine);
        }
    }
}
