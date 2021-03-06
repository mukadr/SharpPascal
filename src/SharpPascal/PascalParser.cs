﻿using SharpPascal.Parsing;
using System.Collections.Generic;
using System.Linq;
using static SharpPascal.Parsing.ParserFactory;

namespace SharpPascal
{
    public class PascalParser
    {
        private readonly Parser<Unit> _parser;

        public PascalParser()
        {
            _parser = BuildPascalParser();
        }

        public Unit Parse(string sourceText)
        {
            return _parser.ParseToCompletion(sourceText);
        }

        private static Parser<Unit> BuildPascalParser()
        {
            var whitespace =
                OneOrMore(
                    Symbol(' ')
                    .Or(Symbol('\t'))
                    .Or(Symbol('\n'))
                    .Or(Symbol('\r')));

            var multilineComment =
               Symbol('{').And(Until('}').Or(new Parser<string>(_ =>
                    throw new ParseException("expected '}' before end of source"))));

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            var letter =
                Symbol('_')
                .Or(Symbol('a', 'z'))
                .Or(Symbol('A', 'Z'));

            var digit =
                Symbol('0', '9');

            Parser<(string text, Location location)> Op(string lexem) =>
                Text(lexem).Map((text, location) => (text, location)).Skip(blank);

            var assign = Op(":=");
            var add = Op("+");
            var sub = Op("-");
            var mul = Op("*");
            var eq = Op("=");
            var ne = Op("<>");
            var le = Op("<=");
            var ge = Op(">=");
            var lt = Op("<");
            var gt = Op(">");
            var lparen = Op("(");
            var rparen = Op(")");
            var colon = Op(":");
            var semi = Op(";");
            var comma = Op(",");
            var dot = Op(".");

            Parser<(string text, Location location)> Kw(string lexem) =>
                Text(lexem, true).Bind((text, location) =>
                    SNot(letter.Or(digit)).Map((_, location) =>
                        (text, location)))
                .Skip(blank);

            var begin = Kw("begin");
            var div = Kw("div");
            var @do = Kw("do");
            var @else = Kw("else");
            var end = Kw("end");
            var @if = Kw("if");
            var mod = Kw("mod");
            var then = Kw("then");
            var var = Kw("var");
            var @while = Kw("while");

            var keyword =
                begin
                .Or(div)
                .Or(@do)
                .Or(@else)
                .Or(end)
                .Or(@if)
                .Or(then)
                .Or(var)
                .Or(@while);

            var id =
                SNot(keyword)
                .And(letter.Bind((l, location) =>
                    ZeroOrMore(letter.Or(digit)).Map(ld =>
                        (text: l + ld, location: location))))
                .Skip(blank);

            var integer =
                OneOrMore(digit).Map<Expression>((value, location) =>
                    new IntegerExpression(int.Parse(value), location))
                .Skip(blank);

            var @string =
                Symbol('\'')
                .And(Until('\'')).Map<Expression>((value, location) =>
                    new StringExpression(value.Substring(0, value.Length - 1), location))
                .Skip(blank);

            var variable =
                id.Map<Expression>(idToken =>
                    new VarExpression(idToken.text, idToken.location));

            var expression =
                Forward<Expression>();

            var arguments =
                expression.Bind(arg =>
                    ZeroOrMore(
                        comma.And(expression)).Map(args =>
                        {
                            args.Insert(0, arg);
                            return args;
                        }))
                .Or(Constant(new List<Expression>()));

            var call =
                id.Bind(idToken =>
                    lparen.And(arguments).Bind(args =>
                        rparen.Map<Expression>(_ =>
                            new CallExpression(idToken.text, args, idToken.location))));

            var factor =
                integer
                .Or(@string)
                .Or(call)
                .Or(variable)
                .Or(lparen.And(expression).Bind(expr => rparen.And(Constant(expr))));

            Parser<Expression> BinaryOperator(
                Parser<Expression> expr,
                Parser<(string text, Location location)> op)
            =>
                expr.Bind(first =>
                    ZeroOrMore(
                        op.Bind(opToken =>
                            expr.Map(right => (opToken, right))))
                    .Map(operatorTerms =>
                        operatorTerms.Aggregate(first, (left, ot) =>
                            BinaryExpression.CreateInstance(
                                left,
                                ot.opToken.text,
                                ot.right,
                                ot.opToken.location))));

            var mulExpression =
                BinaryOperator(factor, mul.Or(div).Or(mod));

            var addExpression =
                BinaryOperator(mulExpression, add.Or(sub));

            var ltExpression =
                BinaryOperator(addExpression, le.Or(ge).Or(lt).Or(gt));

            var eqExpression =
                BinaryOperator(ltExpression, eq.Or(ne));

            expression.Parse =
                eqExpression.Parse;

            var statement =
                Forward<Statement>();

            var ifStatement =
                @if.Bind(ifToken =>
                    expression.Bind(expr =>
                        then.And(statement).Bind(trueStmt =>
                            Maybe(@else.And(statement)).Map<Statement>(falseStmt =>
                                new IfStatement(expr, trueStmt, falseStmt, ifToken.location)))));

            var whileStatement =
                @while.Bind(whileToken =>
                    expression.Bind(expr =>
                        @do.And(statement).Map<Statement>(stmt =>
                            new WhileStatement(expr, stmt, whileToken.location))));

            var assignmentStatement =
                id.Bind(idToken =>
                    assign.And(expression).Map<Statement>(expr =>
                        new AssignmentStatement(idToken.text, expr, idToken.location)));

            var procedureStatement =
                id.Bind(idToken =>
                    Maybe(lparen.And(arguments).Bind(args => rparen.Map(_ => args))).Map<Statement>(args =>
                        new ProcedureStatement(new CallExpression(idToken.text, args, idToken.location))));

            var compoundStatement =
                begin.And(
                    ZeroOrMore(
                        ZeroOrMore(semi)
                        .And(statement))
                    .Bind(stmts =>
                        ZeroOrMore(semi)
                        .And(end)
                        .And(Constant<Statement>(new CompoundStatement(stmts)))));

            statement.Parse =
                compoundStatement
                .Or(ifStatement)
                .Or(whileStatement)
                .Or(assignmentStatement)
                .Or(procedureStatement)
                .Parse;

            var varDeclaration =
                id.Bind(name =>
                    colon.And(id).Bind(type =>
                        semi.And(Constant(new VarDeclaration(name.text, type.text, name.location)))));

            var varSection =
                var.And(varDeclaration).Bind(first =>
                    ZeroOrMore(varDeclaration).Map(vars =>
                    {
                        vars.Insert(0, first);
                        return vars;
                    }));

            var declarations =
                varSection;

            var program =
                Maybe(blank)
                .And(Maybe(declarations)).Bind(decls =>
                    compoundStatement.Bind(main =>
                        dot.Map(_ => new Unit(main, decls))));

            return program;
        }
    }
}
