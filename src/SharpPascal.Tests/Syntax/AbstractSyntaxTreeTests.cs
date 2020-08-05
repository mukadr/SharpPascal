using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;

namespace SharpPascal.Tests
{
    [TestClass]
    public class AbstractSyntaxTreeTests
    {
        [TestMethod]
        public void BinaryExpression_Equals_Test()
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

        [TestMethod]
        public void CallExpression_Equals_Test()
        {
            var expr1 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20),
                    new AddExpression(new IntegerExpression(15), new IntegerExpression(30))
                });

            var expr2 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20),
                    new AddExpression(new IntegerExpression(15), new IntegerExpression(40))
                });

            var expr3 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20),
                    new SubExpression(new IntegerExpression(15), new IntegerExpression(40))
                });

            var expr4 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20),
                    new AddExpression(new IntegerExpression(15), new IntegerExpression(30))
                });

            Assert.AreNotEqual(expr1, expr2);
            Assert.AreNotEqual(expr1, expr3);
            Assert.AreNotEqual(expr2, expr3);
            Assert.AreEqual(expr1, expr4);
        }
    }
}
