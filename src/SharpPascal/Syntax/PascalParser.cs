﻿using SharpPascal.Syntax.Parsing;
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

            Parser<(string text, Location location)> parseOperator(string text) =>
                Text(text)
                .Map((_, line) => (text, new Location(line)))
                .Skip(blank);

            Parser<(string text, Location location)> parseKeyword(string text) =>
                Text(text)
                .And(Not(letter.Or(digit)))
                .Map((_, line) => (text, new Location(line)))
                .Skip(blank);

            var add =
                parseOperator("+");

            var sub =
                parseOperator("-");

            var mul =
                parseOperator("*");

            var div =
                parseKeyword("div");

            var lparen =
                parseOperator("(");

            var rparen =
                parseOperator(")");

            var comma =
                parseOperator(",");

            var dot =
                parseOperator(".");

            var semi =
                parseOperator(";");

            var begin =
                parseKeyword("begin");

            var end =
                parseKeyword("end");

            var keyword =
                begin
                .Or(end)
                .Or(div);

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
                .Or(lparen.And(expression).Bind(e => rparen.And(Constant(e))));

            Parser<Expression> parseBinaryExpression(Parser<Expression> expr, Parser<(string text, Location location)> op) =>
                expr.Bind(first =>
                    ZeroOrMore(op.Bind(op =>
                        expr.Map(right =>
                            (op, right)))).Map(operatorTerms =>
                                operatorTerms.Aggregate(first, (left, ot) =>
                                    BinaryExpression.CreateInstance(left, ot.op.text, ot.right, ot.op.location))));

            var mulExpression =
                parseBinaryExpression(factor, mul.Or(div));

            var addExpression =
                parseBinaryExpression(mulExpression, add.Or(sub));

            expression.Parse =
                addExpression.Parse;

            var expressionStatement =
                expression.Bind(expr => semi.And(Constant<Statement>(new ExpressionStatement(expr, expr.Location))));

            var statement =
                expressionStatement;

            var program =
                Maybe(blank).And(
                    begin.And(Maybe(statement))).Bind(stmt =>
                        end.And(dot).Map(_ => stmt));

            return program.ParseToCompletion(text);
        }
    }
}
