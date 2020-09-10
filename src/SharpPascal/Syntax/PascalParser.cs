using SharpPascal.Syntax.Parsing;
using System.Collections.Generic;
using System.Linq;
using static SharpPascal.Syntax.Parsing.ParserFactory;

namespace SharpPascal.Syntax
{
    public static class PascalParser
    {
        public static Unit? Parse(string text)
        {
            var whitespace =
                Regex("[ \t\n\r]+");

            var multilineComment =
                Regex("{")
                .And(Regex("[^}]*}")
                .Or(new Parser<string>(_ => throw new ParseException("expected '}' before end of source"))));

            var blank =
                OneOrMore(whitespace.Or(multilineComment));

            Parser<(string text, Location location)> op(string text) =>
                Regex(text)
                .Map((text, location) => (text, location))
                .Skip(blank);

            var assign = op(":=");
            var add = op("\\+");
            var sub = op("-");
            var mul = op("\\*");
            var eq = op("=");
            var ne = op("<>");
            var le = op("<=");
            var ge = op(">=");
            var lt = op("<");
            var gt = op(">");
            var lparen = op("\\(");
            var rparen = op("\\)");
            var colon = op(":");
            var semi = op(";");
            var comma = op(",");
            var dot = op("\\.");

            Parser<(string text, Location location)> kw(string text) =>
                Regex(text + "\\b", ignoreCase: true)
                .Map((text, location) => (text, location))
                .Skip(blank);

            var begin = kw("begin");
            var div = kw("div");
            var @do = kw("do");
            var @else = kw("else");
            var end = kw("end");
            var @if = kw("if");
            var mod = kw("mod");
            var then = kw("then");
            var @var = kw("var");
            var @while = kw("while");

            var keyword =
                begin
                .Or(div)
                .Or(@do)
                .Or(@else)
                .Or(end)
                .Or(@if)
                .Or(then)
                .Or(@var)
                .Or(@while);

            var id =
                SNot(keyword)
                .And(Regex("[a-zA-Z_][a-zA-Z_0-9]*"))
                .Map((text, location) => (text, location))
                .Skip(blank);

            var integer =
                Regex("[0-9]+")
                .Map<Expression>((value, location) => new IntegerExpression(int.Parse(value), location))
                .Skip(blank);

            var @string =
                Regex("'[^('\n\r)]*'")
                .Map<Expression>((value, location) => new StringExpression(value.Substring(1, value.Length - 2), location))
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
                .Or(@string)
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
                binaryOperator(factor, mul.Or(div).Or(mod));

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

            var whileStatement =
                @while.Bind(@while =>
                    expression.Bind(expr =>
                        @do.And(statement.Map<Statement>(stmt =>
                            new WhileStatement(expr, stmt, @while.location)))));

            var assignmentStatement =
                id.Bind(id =>
                    assign.And(expression.Map<Statement>(expr =>
                        new AssignmentStatement(id.text, expr, id.location))));

            var procedureStatement =
                id.Bind(id =>
                    Maybe(lparen.And(args.Bind(args => rparen.Map(_ => args))))
                    .Map<Statement>(args =>
                        new ProcedureStatement(new CallExpression(id.text, args, id.location))));

            var compoundStatement =
                begin.And(
                    ZeroOrMore(
                        ZeroOrMore(semi).And(statement)).Bind(stmts =>
                            ZeroOrMore(semi).And(end.And(Constant<Statement>(new CompoundStatement(stmts))))));

            statement.Parse =
                compoundStatement
                .Or(ifStatement)
                .Or(whileStatement)
                .Or(assignmentStatement)
                .Or(procedureStatement)
                .Parse;

            var varDeclaration =
                id.Bind(name =>
                    colon.And(id.Bind(type =>
                        semi.And(Constant(new VarDeclaration(name.text, type.text, name.location))))));

            var varSection =
                @var.And(varDeclaration.Bind(first =>
                    ZeroOrMore(varDeclaration).Map(vars =>
                    {
                        vars.Insert(0, first);
                        return vars;
                    })));

            var declarations =
                varSection;

            var program =
                Maybe(blank)
                .And(Maybe(declarations).Bind(decls =>
                    compoundStatement.Bind(main =>
                        dot.Map(_ => new Unit(main, decls)))));

            return program.ParseToCompletion(text);
        }
    }
}
