using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class VisitorTests
    {
        [TestMethod]
        public void Visitor_Visits_AllNodes()
        {
            const string source = @"
                var
                    X: Integer;
                    Y: Integer;
                begin
                    X := (100 div 10) * 15;
                    Y := X + X mod 4;

                    if X > 200 then
                        Y := Y - 10;

                    if Y < 100 then
                        Y := Y + 10;

                    while Y >= X do
                    begin
                        if Y = X - 5 then
                            Y := Y - 20
                        else
                            Y := Y - 1;

                        if X <= 30 then
                            X := X + 100
                        else if X <> Y + 15 then
                            DoMagicalThing(X, Y * 3, 'foobar!');
                    end;
                end.
            ";

            var unitCount = 0;
            var varDecl = 0;
            var ifCount = 0;
            var whileCount = 0;
            var assignCount = 0;
            var integerCount = 0;
            var stringCount = 0;
            var varCount = 0;
            var addCount = 0;
            var subCount = 0;
            var mulCount = 0;
            var divCount = 0;
            var modCount = 0;
            var equalCount = 0;
            var notEqualCount = 0;
            var lessThanCount = 0;
            var greaterThanCount = 0;
            var lessOrEqualCount = 0;
            var greaterOrEqualCount = 0;
            var callCount = 0;

            var visitor = new Visitor
            {
                VisitUnit = _ => { unitCount++; return true; },
                VisitVarDeclaration = _ => varDecl++,
                VisitIfStatement = _ => { ifCount++; return true; },
                VisitWhileStatement = _ => { whileCount++; return true; },
                VisitAssignmentStatement = _ => { assignCount++; return true; },
                VisitIntegerExpression = _ => integerCount++,
                VisitStringExpression = _ => stringCount++,
                VisitVarExpression = _ => varCount++,
                VisitAddExpression = _ => addCount++,
                VisitSubExpression = _ => subCount++,
                VisitMulExpression = _ => mulCount++,
                VisitDivExpression = _ => divCount++,
                VisitModExpression = _ => modCount++,
                VisitEqualExpression = _ => equalCount++,
                VisitNotEqualExpression = _ => notEqualCount++,
                VisitLessThanExpression = _ => lessThanCount++,
                VisitGreaterThanExpression = _ => greaterThanCount++,
                VisitLessOrEqualExpression = _ => lessOrEqualCount++,
                VisitGreaterOrEqualExpression = _ => greaterOrEqualCount++,
                VisitCallExpression = _ => { callCount++; return true; }
            };

            var tree = PascalParser.Parse(source);

            tree.Visit(visitor);

            Assert.AreEqual(unitCount, 1);
            Assert.AreEqual(varDecl, 2);
            Assert.AreEqual(ifCount, 5);
            Assert.AreEqual(whileCount, 1);
            Assert.AreEqual(assignCount, 7);
            Assert.AreEqual(integerCount, 15);
            Assert.AreEqual(stringCount, 1);
            Assert.AreEqual(varCount, 18);
            Assert.AreEqual(addCount, 4);
            Assert.AreEqual(subCount, 4);
            Assert.AreEqual(mulCount, 2);
            Assert.AreEqual(divCount, 1);
            Assert.AreEqual(modCount, 1);
            Assert.AreEqual(equalCount, 1);
            Assert.AreEqual(notEqualCount, 1);
            Assert.AreEqual(lessThanCount, 1);
            Assert.AreEqual(greaterThanCount, 1);
            Assert.AreEqual(lessOrEqualCount, 1);
            Assert.AreEqual(greaterOrEqualCount, 1);
            Assert.AreEqual(callCount, 1);
        }
    }
}
