using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;
using SharpPascal.Syntax.Parsing;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class PascalParserTests
    {
        [TestMethod]
        public void Skip_Whitespace_Succeeds()
        {
            var source = "\t\t\n\n\r\r     \r\r\n\r    \t \t \n begin end.";

            PascalParser.Parse(source);
        }

        [TestMethod]
        public void Skip_MultilineComment_Succeeds()
        {
            var source = @"
                begin
                {
                    Hello World!
                } end.";

            PascalParser.Parse(source);
        }

        [TestMethod]
        public void Skip_UnterminatedMultilineComment_ThrowsParseException()
        {
            var source = @"
                {
                    Hello World!
            ";

            ParseException? parseException = null;
            try
            {
                PascalParser.Parse(source);
            }
            catch (ParseException ex)
            {
                parseException = ex;
            }

            Assert.IsNotNull(parseException);
            Assert.IsTrue(parseException?.Message?.Contains("expected '}' before end of source") == true);
        }

        [TestMethod]
        public void Parse_IntegerExpression_Succeeds()
        {
            var source = @"
                begin
                    x := 150;
                end.
            ";

            var tree = PascalParser.Parse(source);

            var expected = new UnitSyntax(new CompoundStatementSyntax(new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(150))));

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);

            dynamic dt = tree!;
            Assert.AreEqual(3, dt.Main.Statements[0].Location.Line);
        }

        [TestMethod]
        public void Parse_StringExpression_Succeeds()
        {
            var source = @"
                begin
                    x := 'Hello World!';
                end.
            ";

            var tree = PascalParser.Parse(source);

            var expected = new UnitSyntax(new CompoundStatementSyntax(new AssignmentStatementSyntax("x", new StringExpressionSyntax("Hello World!"))));

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);

            dynamic dt = tree!;
            Assert.AreEqual(3, dt.Main.Statements[0].Location.Line);
        }

        [TestMethod]
        public void Parse_VarExpression_Succeeds()
        {
            var source = @"
                begin
                    division := beta;
                end.
            ";

            var tree = PascalParser.Parse(source);

            var expected = new UnitSyntax(new CompoundStatementSyntax(new AssignmentStatementSyntax("division", new VarExpressionSyntax("beta"))));

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);

            dynamic dt = tree!;
            Assert.AreEqual(3, dt.Main.Statements[0].Location.Line);
        }

        [TestMethod]
        public void Parse_ComputesLocation_Correctly()
        {
            var source = @"
                begin
                    result :=
                        (
                        15
                        + alpha)

                        div
                        2;
                end.
            ";

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);

            dynamic dt = tree!;
            Assert.AreEqual(3, dt.Main.Statements[0].Location.Line);
            Assert.AreEqual(6, dt.Main.Statements[0].Expression.Left.Location.Line);
            Assert.AreEqual(5, dt.Main.Statements[0].Expression.Left.Left.Location.Line);
            Assert.AreEqual(6, dt.Main.Statements[0].Expression.Left.Right.Location.Line);
            Assert.AreEqual(8, dt.Main.Statements[0].Expression.Location.Line);
            Assert.AreEqual(9, dt.Main.Statements[0].Expression.Right.Location.Line);
        }

        [TestMethod]
        public void Parse_BinaryExpression_Succeeds()
        {
            var source = @"
                BEGIN
                    Result := ((20 + Func(15, 18 * (Alpha MOD 3))) DIV Beta) < 12 <> (1 = 1);
                END.
            ";

            var expected = new UnitSyntax(new CompoundStatementSyntax(
                new AssignmentStatementSyntax(
                    "result",
                    new NotEqualExpressionSyntax(
                        new LessThanExpressionSyntax(
                            new DivExpressionSyntax(
                                new AddExpressionSyntax(
                                    new IntegerExpressionSyntax(20),
                                    new CallExpressionSyntax("func",
                                        new IntegerExpressionSyntax(15),
                                        new MulExpressionSyntax(
                                            new IntegerExpressionSyntax(18),
                                            new ModExpressionSyntax(
                                                new VarExpressionSyntax("alpha"),
                                                new IntegerExpressionSyntax(3))))
                                ),
                                new VarExpressionSyntax("beta")),
                            new IntegerExpressionSyntax(12)),
                        new EqualExpressionSyntax(
                            new IntegerExpressionSyntax(1),
                            new IntegerExpressionSyntax(1))))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_IfStatement_Succeeds()
        {
            var source = @"
                begin
                    if 100 > 50 then
                        if 15 = 15 then
                            x := 20
                        else
                            x := 15;
                end.
            ";

            var expected = new UnitSyntax(new CompoundStatementSyntax(
                new IfStatementSyntax(
                    new GreaterThanExpressionSyntax(
                        new IntegerExpressionSyntax(100),
                        new IntegerExpressionSyntax(50)),
                    new IfStatementSyntax(
                        new EqualExpressionSyntax(
                            new IntegerExpressionSyntax(15),
                            new IntegerExpressionSyntax(15)),
                        new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(20)),
                        new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(15))))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_WhileStatement_Succeeds()
        {
            var source = @"
                begin
                    while I < 10 do
                        I := I + 1;
                end.
            ";

            var expected = new UnitSyntax(new CompoundStatementSyntax(
                new WhileStatementSyntax(
                    new LessThanExpressionSyntax(
                        new VarExpressionSyntax("i"),
                        new IntegerExpressionSyntax(10)),
                    new AssignmentStatementSyntax("i",
                        new AddExpressionSyntax(
                            new VarExpressionSyntax("i"),
                            new IntegerExpressionSyntax(1))))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_ProcedureStatement_Succeeds()
        {
            var source = @"
                begin
                    Inc(I);
                    WriteLn;
                end.
            ";

            var expected = new UnitSyntax(new CompoundStatementSyntax(
                new ProcedureStatementSyntax(
                    new CallExpressionSyntax("inc", new VarExpressionSyntax("i"))),
                new ProcedureStatementSyntax(
                    new CallExpressionSyntax("writeln"))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_CompoundStatement_Succeeds()
        {
            var source = @"
                begin
                    x := 10;
                    y := 15;
                    if x + y < 30 then
                        x := 20;
                    z := x + y;
                end.
            ";

            var expected = new UnitSyntax(new CompoundStatementSyntax(
                new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(10)),
                new AssignmentStatementSyntax("y", new IntegerExpressionSyntax(15)),
                new IfStatementSyntax(
                    new LessThanExpressionSyntax(
                        new AddExpressionSyntax(
                            new VarExpressionSyntax("x"),
                            new VarExpressionSyntax("y")),
                        new IntegerExpressionSyntax(30)),
                    new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(20))),
                new AssignmentStatementSyntax("z",
                    new AddExpressionSyntax(
                        new VarExpressionSyntax("x"),
                        new VarExpressionSyntax("y")))));

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_VarDeclaration_Succeeds()
        {
            var source = @"
                var
                    x: integer;
                    y: integer;
                    z: integer;

                begin
                    x := 10;
                    y := 20;
                    z := x * y
                end.
            ";

            var expected = new UnitSyntax(
                new CompoundStatementSyntax(
                    new AssignmentStatementSyntax("x", new IntegerExpressionSyntax(10)),
                    new AssignmentStatementSyntax("y", new IntegerExpressionSyntax(20)),
                    new AssignmentStatementSyntax("z",
                        new MulExpressionSyntax(
                            new VarExpressionSyntax("x"),
                            new VarExpressionSyntax("y")))),
                new VarDeclarationSyntax("x", "integer"),
                new VarDeclarationSyntax("y", "integer"),
                new VarDeclarationSyntax("z", "integer"));

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_Accepts_SampleProgram1()
        {
            var source = @"
                var
                    x: Integer;
                    xTimesTwo: Integer;

                begin
                    Write('Enter a number: ');

                    Read(x);

                    xTimesTwo := x * 2;

                    WriteLn('You entered ', x, ', and ', x, ' multiplied by 2 is ', xTimesTwo, '.');
                end.
            ";

            var expected = new UnitSyntax(
                new CompoundStatementSyntax(
                    new ProcedureStatementSyntax(new CallExpressionSyntax("write", new StringExpressionSyntax("Enter a number: "))),
                    new ProcedureStatementSyntax(new CallExpressionSyntax("read", new VarExpressionSyntax("x"))),
                    new AssignmentStatementSyntax("xTimesTwo", new MulExpressionSyntax(new VarExpressionSyntax("x"), new IntegerExpressionSyntax(2))),
                    new ProcedureStatementSyntax(new CallExpressionSyntax("writeln",
                        new StringExpressionSyntax("You entered "),
                        new VarExpressionSyntax("x"),
                        new StringExpressionSyntax(", and "),
                        new VarExpressionSyntax("x"),
                        new StringExpressionSyntax(" multiplied by 2 is "),
                        new VarExpressionSyntax("xTimesTwo"),
                        new StringExpressionSyntax(".")))),
                new VarDeclarationSyntax("x", "integer"),
                new VarDeclarationSyntax("xTimesTwo", "integer"));

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void Parse_Accepts_SampleProgram2()
        {
            var source = @"
                var
                    Value: Integer;
                    I: Integer;
                    Factorial: Integer;

                begin
                    Write('Enter a number: ');

                    Read(Value);

                    I := Value;
                    Factorial := 1;
                    while I > 0 do
                    begin
                        Factorial := Factorial * I;
                        Dec(I);
                    end;

                    WriteLn('The factorial of ', Value, ' is ', Factorial);
                end.
            ";

            var expected = new UnitSyntax(
                new CompoundStatementSyntax(
                    new ProcedureStatementSyntax(new CallExpressionSyntax("write", new StringExpressionSyntax("Enter a number: "))),
                    new ProcedureStatementSyntax(new CallExpressionSyntax("read", new VarExpressionSyntax("value"))),
                    new AssignmentStatementSyntax("i", new VarExpressionSyntax("value")),
                    new AssignmentStatementSyntax("factorial", new IntegerExpressionSyntax(1)),
                    new WhileStatementSyntax(
                        new GreaterThanExpressionSyntax(new VarExpressionSyntax("i"), new IntegerExpressionSyntax(0)),
                        new CompoundStatementSyntax(
                            new AssignmentStatementSyntax("factorial", new MulExpressionSyntax(new VarExpressionSyntax("factorial"), new VarExpressionSyntax("i"))),
                            new ProcedureStatementSyntax(new CallExpressionSyntax("dec", new VarExpressionSyntax("i"))))),
                    new ProcedureStatementSyntax(new CallExpressionSyntax("writeln",
                        new StringExpressionSyntax("The factorial of "),
                        new VarExpressionSyntax("value"),
                        new StringExpressionSyntax(" is "),
                        new VarExpressionSyntax("factorial")))),
                new VarDeclarationSyntax("value", "integer"),
                new VarDeclarationSyntax("i", "integer"),
                new VarDeclarationSyntax("factorial", "integer"));

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);
        }
    }
}
