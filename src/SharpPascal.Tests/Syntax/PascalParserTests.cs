using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;
using SharpPascal.Syntax.Parsing;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class PascalParserTests
    {
        [TestMethod]
        public void SkipWhiteTest()
        {
            var source = "\t\t\n\n\r\r     \r\r\n\r    \t \t \n begin end.";

            PascalParser.Parse(source);

            source = @"
                begin
                {
                    Hello World!
                } end.";

            PascalParser.Parse(source);
        }

        [TestMethod]
        public void UnterminatedCommentTest()
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
        public void ParseNumberTest()
        {
            var source = @"
                begin
                    150;
                end.
            ";

            var tree = PascalParser.Parse(source);

            var expected = new ExpressionStatement(new IntegerExpression(150));

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(3, tree?.Location?.Line);
            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void ParseIdTest()
        {
            var source = @"
                begin
                    division;
                end.
            ";

            var tree = PascalParser.Parse(source);

            var expected = new ExpressionStatement(new VarExpression("division"));

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(3, tree?.Location?.Line);
            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void ParseBinaryExpressionTest()
        {
            var source = @"
                begin
                    (
                        20
                        +
                        func(15, 18 * 2)
                    )
                    div
                    beta
                    < 12

                    <>

                    1 >= 2;
                end.
            ";

            var expected =
                new ExpressionStatement(
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
                                            new IntegerExpression(2)
                                        )
                                    })
                                ),
                                new VarExpression("beta")),
                            new IntegerExpression(12)),
                        new GreaterOrEqualExpression(
                            new IntegerExpression(1),
                            new IntegerExpression(2))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);

            if (tree != null)
            {
                dynamic dt = tree;

                Assert.AreEqual(8, dt.Expression.Left.Left.Location.Line);
                Assert.AreEqual(5, dt.Expression.Left.Left.Left.Location.Line);
                Assert.AreEqual(9, dt.Expression.Left.Left.Right.Location.Line);
                Assert.AreEqual(4, dt.Expression.Left.Left.Left.Left.Location.Line);
                Assert.AreEqual(6, dt.Expression.Left.Left.Left.Right.Location.Line);
            }
        }

        [TestMethod]
        public void ParseIfStatementTest()
        {
            var source = @"
                begin
                    if 100 > 50 then
                        if 15 = 15 then
                            20;
                        else
                            15;
                end.
            ";

            var expected =
                new IfStatement(
                    new GreaterThanExpression(
                        new IntegerExpression(100),
                        new IntegerExpression(50)),
                    new IfStatement(
                        new EqualExpression(
                            new IntegerExpression(15),
                            new IntegerExpression(15)),
                        new ExpressionStatement(new IntegerExpression(20)),
                        new ExpressionStatement(new IntegerExpression(15))));

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }
    }
}
