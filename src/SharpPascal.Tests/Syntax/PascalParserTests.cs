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

            var expected = new Unit(new CompoundStatement(new AssignmentStatement("x", new IntegerExpression(150))));

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

            var expected = new Unit(new CompoundStatement(new AssignmentStatement("division", new VarExpression("beta"))));

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

            var expected = new Unit(new CompoundStatement(
                new AssignmentStatement(
                    "result",
                    new NotEqualExpression(
                        new LessThanExpression(
                            new DivExpression(
                                new AddExpression(
                                    new IntegerExpression(20),
                                    new CallExpression("func", new Expression[]
                                    {
                                        new IntegerExpression(15),
                                        new MulExpression(
                                            new IntegerExpression(18),
                                            new ModExpression(
                                                new VarExpression("alpha"),
                                                new IntegerExpression(3)
                                            )
                                        )
                                    })
                                ),
                                new VarExpression("beta")),
                            new IntegerExpression(12)),
                        new EqualExpression(
                            new IntegerExpression(1),
                            new IntegerExpression(1))))));

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

            var expected = new Unit(new CompoundStatement(
                new IfStatement(
                    new GreaterThanExpression(
                        new IntegerExpression(100),
                        new IntegerExpression(50)),
                    new IfStatement(
                        new EqualExpression(
                            new IntegerExpression(15),
                            new IntegerExpression(15)),
                        new AssignmentStatement("x", new IntegerExpression(20)),
                        new AssignmentStatement("x", new IntegerExpression(15))))));

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

            var expected = new Unit(new CompoundStatement(
                new WhileStatement(
                    new LessThanExpression(
                        new VarExpression("i"),
                        new IntegerExpression(10)),
                    new AssignmentStatement("i",
                        new AddExpression(
                            new VarExpression("i"),
                            new IntegerExpression(1))))));

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

            var expected = new Unit(new CompoundStatement(
                new ProcedureStatement(
                    new CallExpression("inc", new Expression[]
                    {
                        new VarExpression("i")
                    })),
                new ProcedureStatement(
                    new CallExpression("writeln"))));

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

            var expected = new Unit(new CompoundStatement(
                new AssignmentStatement("x", new IntegerExpression(10)),
                new AssignmentStatement("y", new IntegerExpression(15)),
                new IfStatement(
                    new LessThanExpression(
                        new AddExpression(
                            new VarExpression("x"),
                            new VarExpression("y")),
                        new IntegerExpression(30)),
                    new AssignmentStatement("x", new IntegerExpression(20))),
                new AssignmentStatement("z",
                    new AddExpression(
                        new VarExpression("x"),
                        new VarExpression("y")))));

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

            var expected = new Unit(
                new CompoundStatement(
                    new AssignmentStatement("x", new IntegerExpression(10)),
                    new AssignmentStatement("y", new IntegerExpression(20)),
                    new AssignmentStatement("z",
                        new MulExpression(
                            new VarExpression("x"),
                            new VarExpression("y")))),
                new Declaration[]
                {
                    new VarDeclaration("x", "integer"),
                    new VarDeclaration("y", "integer"),
                    new VarDeclaration("z", "integer"),
                });

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree);
            Assert.AreEqual(expected, tree);
        }
    }
}
