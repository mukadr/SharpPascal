﻿using SharpPascal.Syntax.Parsing;
using System.Collections.Generic;
using System.Linq;
using static SharpPascal.Syntax.Parsing.ParserFactory;

namespace SharpPascal.Syntax
{
    public static class PascalParser
    {
        public static UnitSyntax Parse(string sourceText)
        {
            var whitespace =
                Regex("[ \t\n\r]+");

            var multilineComment =
                Regex("{").And(Regex("[^}]*}").Or(new Parser<string>(_ =>
                    throw new ParseException("expected '}' before end of source"))));

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            Parser<(string text, Location location)> Op(string regex) =>
                Regex(regex).Map((text, location) => (text, location)).Skip(blank);

            var assign = Op(":=");
            var add = Op("\\+");
            var sub = Op("-");
            var mul = Op("\\*");
            var eq = Op("=");
            var ne = Op("<>");
            var le = Op("<=");
            var ge = Op(">=");
            var lt = Op("<");
            var gt = Op(">");
            var lparen = Op("\\(");
            var rparen = Op("\\)");
            var colon = Op(":");
            var semi = Op(";");
            var comma = Op(",");
            var dot = Op("\\.");

            Parser<(string text, Location location)> Kw(string name) =>
                Regex(name + "\\b", true).Map((text, location) => (text, location)).Skip(blank);

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
                .And(Regex("[a-zA-Z_][a-zA-Z_0-9]*")).Map((text, location) => (text, location))
                .Skip(blank);

            var integer =
                Regex("[0-9]+").Map<ExpressionSyntax>((value, location) =>
                    new IntegerExpressionSyntax(int.Parse(value), location))
                .Skip(blank);

            var @string =
                Regex("'[^('\n\r)]*'").Map<ExpressionSyntax>((value, location) =>
                    new StringExpressionSyntax(value.Substring(1, value.Length - 2), location))
                .Skip(blank);

            var variable =
                id.Map<ExpressionSyntax>(idToken =>
                    new VarExpressionSyntax(idToken.text, idToken.location));

            var expression =
                Forward<ExpressionSyntax>();

            var arguments =
                expression.Bind(arg =>
                    ZeroOrMore(
                        comma.And(expression)).Map(args =>
                        {
                            args.Insert(0, arg);
                            return args;
                        }))
                .Or(Constant(new List<ExpressionSyntax>()));

            var call =
                id.Bind(idToken =>
                    lparen.And(arguments).Bind(args =>
                        rparen.Map<ExpressionSyntax>(_ =>
                            new CallExpressionSyntax(idToken.text, args, idToken.location))));

            var factor =
                integer
                .Or(@string)
                .Or(call)
                .Or(variable)
                .Or(lparen.And(expression).Bind(expr => rparen.And(Constant(expr))));

            Parser<ExpressionSyntax> BinaryOperator(
                Parser<ExpressionSyntax> expr,
                Parser<(string text, Location location)> op)
            =>
                expr.Bind(first =>
                    ZeroOrMore(
                        op.Bind(opToken =>
                            expr.Map(right => (opToken, right))))
                    .Map(operatorTerms =>
                        operatorTerms.Aggregate(first, (left, ot) =>
                            BinaryExpressionSyntax.CreateInstance(
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
                Forward<StatementSyntax>();

            var ifStatement =
                @if.Bind(ifToken =>
                    expression.Bind(expr =>
                        then.And(statement).Bind(trueStmt =>
                            Maybe(@else.And(statement)).Map<StatementSyntax>(falseStmt =>
                                new IfStatementSyntax(expr, trueStmt, falseStmt, ifToken.location)))));

            var whileStatement =
                @while.Bind(whileToken =>
                    expression.Bind(expr =>
                        @do.And(statement).Map<StatementSyntax>(stmt =>
                            new WhileStatementSyntax(expr, stmt, whileToken.location))));

            var assignmentStatement =
                id.Bind(idToken =>
                    assign.And(expression).Map<StatementSyntax>(expr =>
                        new AssignmentStatementSyntax(idToken.text, expr, idToken.location)));

            var procedureStatement =
                id.Bind(idToken =>
                    Maybe(lparen.And(arguments).Bind(args => rparen.Map(_ => args))).Map<StatementSyntax>(args =>
                        new ProcedureStatementSyntax(new CallExpressionSyntax(idToken.text, args, idToken.location))));

            var compoundStatement =
                begin.And(
                    ZeroOrMore(
                        ZeroOrMore(semi)
                        .And(statement))
                    .Bind(stmts =>
                        ZeroOrMore(semi)
                        .And(end)
                        .And(Constant<StatementSyntax>(new CompoundStatementSyntax(stmts)))));

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
                        semi.And(Constant(new VarDeclarationSyntax(name.text, type.text, name.location)))));

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
                        dot.Map(_ => new UnitSyntax(main, decls))));

            return program.ParseToCompletion(sourceText);
        }
    }
}
