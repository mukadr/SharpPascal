using Xunit;

namespace SharpPascal.Tests
{
    public class VisitorTests : IClassFixture<PascalParserFixture>
    {
        private readonly PascalParser _parser;

        public VisitorTests(PascalParserFixture pascalParserFixture)
        {
            _parser = pascalParserFixture.Parser;
        }

        [Fact]
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

            var tree = _parser.Parse(source);

            tree.Visit(visitor);

            Assert.Equal(1, unitCount);
            Assert.Equal(2, varDecl);
            Assert.Equal(5, ifCount);
            Assert.Equal(1, whileCount);
            Assert.Equal(7, assignCount);
            Assert.Equal(15, integerCount);
            Assert.Equal(1, stringCount);
            Assert.Equal(18, varCount);
            Assert.Equal(4, addCount);
            Assert.Equal(4, subCount);
            Assert.Equal(2, mulCount);
            Assert.Equal(1, divCount);
            Assert.Equal(1, modCount);
            Assert.Equal(1, equalCount);
            Assert.Equal(1, notEqualCount);
            Assert.Equal(1, lessThanCount);
            Assert.Equal(1, greaterThanCount);
            Assert.Equal(1, lessOrEqualCount);
            Assert.Equal(1, greaterOrEqualCount);
            Assert.Equal(1, callCount);
        }
    }
}
