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
            var source = new Source("\t\t\n\n\r\r     \r\r\n\r    \t \t \n");

            var (tree, line) = PascalParser.Parse(source);

            Assert.IsNull(tree);
            Assert.AreEqual(5, line);

            source = new Source(@"
                {
                    Hello World!
                }");

            (tree, line) = PascalParser.Parse(source);

            Assert.IsNull(tree);
            Assert.AreEqual(4, line);
        }

        [TestMethod]
        public void UnterminatedCommentTest()
        {
            var source = new Source(@"
                {
                    Hello World!
                ");

            Assert.ThrowsException<ParseException>(() => PascalParser.Parse(source));
        }

        [TestMethod]
        public void ParseNumberTest()
        {
            var source = new Source(@"
                150
            ");

            var tree = PascalParser.Parse(source).Tree;

            Assert.IsNotNull(tree);
            Assert.AreEqual(2, tree?.Location.Line);
            Assert.AreEqual(new IntegerExpression(150, new Location(2)), tree);
        }
    }
}
