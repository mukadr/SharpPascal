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
            var loc = new Location(1);

            var expr1 = new AddExpression(new IntegerExpression(100, loc), new IntegerExpression(200, loc), loc);
            var expr2 = new SubExpression(new IntegerExpression(100, loc), new IntegerExpression(200, loc), loc);
            var expr3 = new AddExpression(new IntegerExpression(100, loc), new IntegerExpression(300, loc), loc);
            var expr4 = new AddExpression(new IntegerExpression(100, loc), new IntegerExpression(200, loc), loc);

            Assert.AreNotEqual(expr1, expr2);
            Assert.AreNotEqual(expr1, expr3);
            Assert.AreNotEqual(expr2, expr3);
            Assert.AreEqual(expr1, expr4);
        }

        [TestMethod]
        public void CallExpression_Equals_Test()
        {
            var loc = new Location(1);

            var expr1 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20, loc),
                    new AddExpression(new IntegerExpression(15, loc), new IntegerExpression(30, loc), loc)
                }, loc);

            var expr2 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20, loc),
                    new AddExpression(new IntegerExpression(15, loc), new IntegerExpression(40, loc), loc)
                }, loc);

            var expr3 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20, loc),
                    new SubExpression(new IntegerExpression(15, loc), new IntegerExpression(40, loc), loc)
                }, loc);

            var expr4 = new CallExpression("Func1",
                new Expression[]
                {
                    new IntegerExpression(20, loc),
                    new AddExpression(new IntegerExpression(15, loc), new IntegerExpression(30, loc), loc)
                }, loc);

            Assert.AreNotEqual(expr1, expr2);
            Assert.AreNotEqual(expr1, expr3);
            Assert.AreNotEqual(expr2, expr3);
            Assert.AreEqual(expr1, expr4);
        }
    }
}
