﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var source = "\t\t\n\n\r\r     \r\r\n\r    \t \t \n end";

            var tree = PascalParser.Parse(source);

            var expected = new VarExpression("end");

            Assert.AreEqual(expected, tree);
            Assert.AreEqual(5, tree?.Location?.Line);

            source = @"
                {
                    Hello World!
                } end";

            tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
            Assert.AreEqual(4, tree?.Location?.Line);
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
                150
            ";

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(2, tree?.Location?.Line);
            Assert.AreEqual(new IntegerExpression(150), tree);
        }

        [TestMethod]
        public void ParseIdTest()
        {
            var source = @"
                division
            ";

            var tree = PascalParser.Parse(source);

            Assert.IsNotNull(tree?.Location);
            Assert.AreEqual(2, tree?.Location?.Line);
            Assert.AreEqual(new VarExpression("division"), tree);
        }

        [TestMethod]
        public void ParseBinaryExpressionTest()
        {
            var source = @"20 + alpha * 18";

            var expected = new AddExpression(
                new IntegerExpression(20),
                new MulExpression(
                    new VarExpression("alpha"),
                    new IntegerExpression(18)
                )
            );

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }

        [TestMethod]
        public void ParseParenthesesExpressionTest()
        {
            var source = @"(20 + alpha) * 18";

            var expected = new MulExpression(
                new AddExpression(
                    new IntegerExpression(20),
                    new VarExpression("alpha")
                ),
                new IntegerExpression(18)
            );

            var tree = PascalParser.Parse(source);

            Assert.AreEqual(expected, tree);
        }
    }
}