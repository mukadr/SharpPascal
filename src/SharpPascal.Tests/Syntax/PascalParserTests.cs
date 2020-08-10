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

            Assert.ThrowsException<ParseException>(() => PascalParser.Parse(source));
        }

        [TestMethod]
        public void ParseNumberTest()
        {
            var source = @"
                begin
                    150
                end.
            ";

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(3, tree?.Location?.Line);
            Assert.AreEqual(new IntegerExpression(150), tree);
        }

        [TestMethod]
        public void ParseIdTest()
        {
            var source = @"
                begin
                    division
                end.
            ";

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(3, tree?.Location?.Line);
            Assert.AreEqual(new VarExpression("division"), tree);
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
                end.
            ";

            var expected = new DivExpression(
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
                new VarExpression("beta")
            );

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);

            if (tree != null)
            {
                dynamic dt = tree;

                Assert.AreEqual(8, dt.Location.Line);
                Assert.AreEqual(5, dt.Left.Location.Line);
                Assert.AreEqual(9, dt.Right.Location.Line);
                Assert.AreEqual(4, dt.Left.Left.Location.Line);
                Assert.AreEqual(6, dt.Left.Right.Location.Line);
            }
        }
    }
}
