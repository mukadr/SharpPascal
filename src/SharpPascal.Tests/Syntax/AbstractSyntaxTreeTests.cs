using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;

namespace SharpPascal.Tests
{
    [TestClass]
    public class AbstractSyntaxTreeTests
    {
        [TestMethod]
        public void AbstractSyntaxTree_Equals_Test()
        {
            var expr1 = new AddExpression(new IntegerExpression(100), new IntegerExpression(200));
            var expr2 = new SubExpression(new IntegerExpression(100), new IntegerExpression(200));
            var expr3 = new AddExpression(new IntegerExpression(100), new IntegerExpression(300));
            var expr4 = new AddExpression(new IntegerExpression(100), new IntegerExpression(200));

            Assert.AreNotEqual(expr1, expr2);
            Assert.AreNotEqual(expr1, expr3);
            Assert.AreNotEqual(expr2, expr3);
            Assert.AreEqual(expr1, expr4);
        }
    }
}
