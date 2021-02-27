using SharpPascal.Parsing;
using Xunit;

namespace SharpPascal.Tests
{
    public class PascalParserTests : IClassFixture<PascalParserFixture>
    {
        private readonly PascalParser _parser;

        public PascalParserTests(PascalParserFixture pascalParserFixture)
        {
            _parser = pascalParserFixture.Parser;
        }

        [Fact]
        public void Skip_Whitespace_Succeeds()
        {
            const string source = "\t\t\n\n\r\r     \r\r\n\r    \t \t \n begin end.";

            _parser.Parse(source);
        }

        [Fact]
        public void Skip_MultilineComment_Succeeds()
        {
            const string source = @"
                begin
                {
                    Hello World!
                } end.
            ";

            _parser.Parse(source);
        }

        [Fact]
        public void Skip_UnterminatedMultilineComment_ThrowsParseException()
        {
            const string source = @"
                {
                    Hello World!
            ";

            ParseException? parseException = null;
            try
            {
                _parser.Parse(source);
            }
            catch (ParseException ex)
            {
                parseException = ex;
            }

            Assert.NotNull(parseException);
            Assert.Contains("expected '}' before end of source", parseException!.Message);
        }

        [Fact]
        public void Parse_IntegerExpression_Succeeds()
        {
            const string source = @"
                begin
                    x := 150;
                end.
            ";

            var tree = _parser.Parse(source);

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "x",
                            new IntegerExpression(150))));

            Assert.Equal(expected, tree);

            dynamic dt = tree;
            Assert.Equal(3, dt.Main.Statements[0].Location.Line);
        }

        [Fact]
        public void Parse_StringExpression_Succeeds()
        {
            const string source = @"
                begin
                    x := 'Hello World!';
                end.
            ";

            var tree = _parser.Parse(source);

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "x",
                            new StringExpression("Hello World!"))));

            Assert.Equal(expected, tree);

            dynamic dt = tree;
            Assert.Equal(3, dt.Main.Statements[0].Location.Line);
        }

        [Fact]
        public void Parse_VarExpression_Succeeds()
        {
            const string source = @"
                begin
                    division := beta;
                end.
            ";

            var tree = _parser.Parse(source);

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "division",
                            new VarExpression("beta"))));

            Assert.Equal(expected, tree);

            dynamic dt = tree;
            Assert.Equal(3, dt.Main.Statements[0].Location.Line);
        }

        [Fact]
        public void Parse_ComputesLocation_Correctly()
        {
            const string source = @"
                begin
                    result :=
                        (
                        15
                        + alpha)

                        div
                        2;
                end.
            ";

            var tree = _parser.Parse(source);

            dynamic dt = tree;
            Assert.Equal(3, dt.Main.Statements[0].Location.Line);
            Assert.Equal(6, dt.Main.Statements[0].Expression.Left.Location.Line);
            Assert.Equal(5, dt.Main.Statements[0].Expression.Left.Left.Location.Line);
            Assert.Equal(6, dt.Main.Statements[0].Expression.Left.Right.Location.Line);
            Assert.Equal(8, dt.Main.Statements[0].Expression.Location.Line);
            Assert.Equal(9, dt.Main.Statements[0].Expression.Right.Location.Line);
        }

        [Fact]
        public void Parse_BinaryExpression_Succeeds()
        {
            const string source = @"
                BEGIN
                    Result := ((20 + Func(15, 18 * (Alpha MOD 3))) DIV Beta) < 12 <> (1 = 1);
                END.
            ";

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "result",
                            new NotEqualExpression(
                                new LessThanExpression(
                                    new DivExpression(
                                        new AddExpression(
                                            new IntegerExpression(20),
                                            new CallExpression(
                                                "func",
                                                new IntegerExpression(15),
                                                new MulExpression(
                                                    new IntegerExpression(18),
                                                    new ModExpression(
                                                        new VarExpression("alpha"),
                                                        new IntegerExpression(3))))
                                        ),
                                        new VarExpression("beta")),
                                    new IntegerExpression(12)),
                                new EqualExpression(
                                    new IntegerExpression(1),
                                    new IntegerExpression(1))))));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_IfStatement_Succeeds()
        {
            const string source = @"
                begin
                    if 100 > 50 then
                        if 15 = 15 then
                            x := 20
                        else
                            x := 15;
                end.
            ";

            var expected =
                new Unit(
                    new CompoundStatement(
                        new IfStatement(
                            new GreaterThanExpression(
                                new IntegerExpression(100),
                                new IntegerExpression(50)),
                            new IfStatement(
                                new EqualExpression(
                                    new IntegerExpression(15),
                                    new IntegerExpression(15)),
                                new AssignmentStatement(
                                    "x",
                                    new IntegerExpression(20)),
                                new AssignmentStatement(
                                    "x",
                                    new IntegerExpression(15))))));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_WhileStatement_Succeeds()
        {
            const string source = @"
                begin
                    while I < 10 do
                        I := I + 1;
                end.
            ";

            var expected =
                new Unit(
                    new CompoundStatement(
                        new WhileStatement(
                            new LessThanExpression(
                                new VarExpression("i"),
                                new IntegerExpression(10)),
                            new AssignmentStatement(
                                "i",
                                new AddExpression(
                                    new VarExpression("i"),
                                    new IntegerExpression(1))))));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_ProcedureStatement_Succeeds()
        {
            const string source = @"
                begin
                    Inc(I);
                    WriteLn;
                end.
            ";

            var expected =
                new Unit(
                    new CompoundStatement(
                        new ProcedureStatement(
                            new CallExpression(
                                "inc",
                                new VarExpression("i"))),
                        new ProcedureStatement(
                            new CallExpression("writeln"))));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_CompoundStatement_Succeeds()
        {
            const string source = @"
                begin
                    x := 10;
                    y := 15;
                    if x + y < 30 then
                        x := 20;
                    z := x + y;
                end.
            ";

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "x",
                            new IntegerExpression(10)),
                        new AssignmentStatement(
                            "y",
                            new IntegerExpression(15)),
                        new IfStatement(
                            new LessThanExpression(
                                new AddExpression(
                                    new VarExpression("x"),
                                    new VarExpression("y")),
                                new IntegerExpression(30)),
                            new AssignmentStatement(
                                "x",
                                new IntegerExpression(20))),
                        new AssignmentStatement(
                            "z",
                            new AddExpression(
                                new VarExpression("x"),
                                new VarExpression("y")))));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_VarDeclaration_Succeeds()
        {
            const string source = @"
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

            var expected =
                new Unit(
                    new CompoundStatement(
                        new AssignmentStatement(
                            "x",
                            new IntegerExpression(10)),
                        new AssignmentStatement(
                            "y",
                            new IntegerExpression(20)),
                        new AssignmentStatement(
                            "z",
                            new MulExpression(
                                new VarExpression("x"),
                                new VarExpression("y")))),
                    new VarDeclaration("x", "integer"),
                    new VarDeclaration("y", "integer"),
                    new VarDeclaration("z", "integer"));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_Accepts_SampleProgram1()
        {
            const string source = @"
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

            var expected =
                new Unit(
                    new CompoundStatement(
                        new ProcedureStatement(
                            new CallExpression(
                                "write",
                                new StringExpression("Enter a number: "))),
                        new ProcedureStatement(
                            new CallExpression(
                                "read",
                                new VarExpression("x"))),
                        new AssignmentStatement(
                            "xTimesTwo",
                            new MulExpression(
                                new VarExpression("x"),
                                new IntegerExpression(2))),
                        new ProcedureStatement(
                            new CallExpression(
                                "writeln",
                                new StringExpression("You entered "),
                                new VarExpression("x"),
                                new StringExpression(", and "),
                                new VarExpression("x"),
                                new StringExpression(" multiplied by 2 is "),
                                new VarExpression("xTimesTwo"),
                                new StringExpression(".")))),
                    new VarDeclaration("x", "integer"),
                    new VarDeclaration("xTimesTwo", "integer"));

            Assert.Equal(expected, _parser.Parse(source));
        }

        [Fact]
        public void Parse_Accepts_SampleProgram2()
        {
            const string source = @"
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

            var expected =
                new Unit(
                    new CompoundStatement(
                        new ProcedureStatement(
                            new CallExpression(
                                "write",
                                new StringExpression("Enter a number: "))),
                        new ProcedureStatement(
                            new CallExpression(
                                "read",
                                new VarExpression("value"))),
                        new AssignmentStatement(
                            "i",
                            new VarExpression("value")),
                        new AssignmentStatement(
                            "factorial",
                            new IntegerExpression(1)),
                        new WhileStatement(
                            new GreaterThanExpression(
                                new VarExpression("i"),
                                new IntegerExpression(0)),
                            new CompoundStatement(
                                new AssignmentStatement(
                                    "factorial",
                                    new MulExpression(
                                        new VarExpression("factorial"),
                                        new VarExpression("i"))),
                                new ProcedureStatement(
                                    new CallExpression(
                                        "dec",
                                        new VarExpression("i"))))),
                        new ProcedureStatement(
                            new CallExpression(
                                "writeln",
                                new StringExpression("The factorial of "),
                                new VarExpression("value"),
                                new StringExpression(" is "),
                                new VarExpression("factorial")))),
                    new VarDeclaration("value", "integer"),
                    new VarDeclaration("i", "integer"),
                    new VarDeclaration("factorial", "integer"));

            Assert.Equal(expected, _parser.Parse(source));
        }
    }
}
