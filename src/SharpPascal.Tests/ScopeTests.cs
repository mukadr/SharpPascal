using Xunit;

namespace SharpPascal.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void Scope_Equals_Works()
        {
            var integer = new Type("integer");
            var @string = new Type("string");

            var scope1 = new Scope();
            scope1.Add(new Symbol("foo", integer));
            scope1.Add(new Symbol("bar", @string));

            var scope2 = new Scope(scope1);
            scope2.Add(new Symbol("number", integer));

            var scope3 = new Scope();
            scope3.Add(new Symbol("bar", @string));

            var scope4 = new Scope(scope3);
            scope4.Add(new Symbol("number", integer));

            Assert.NotEqual(scope2, scope4);

            scope3.Add(new Symbol("foo", integer));

            Assert.Equal(scope2, scope4);
        }

        [Fact]
        public void Scope_Finds_LocalSymbol()
        {
            var scope1 = new Scope();
            var scope2 = new Scope(scope1);

            var integer = new Type("integer");
            var sym1 = new Symbol("foo", integer);
            var sym2 = new Symbol("bar", integer);

            scope1.Add(sym1);
            scope2.Add(sym2);

            var mustFind1 = scope1.FindLocal(sym1.Name);
            var mustFail1 = scope1.FindLocal(sym2.Name);

            Assert.Equal(sym1, mustFind1);
            Assert.Null(mustFail1);

            var mustFind2 = scope2.FindLocal(sym2.Name);
            var mustFail2 = scope2.FindLocal(sym1.Name);

            Assert.Equal(sym2, mustFind2);
            Assert.Null(mustFail2);
        }

        [Fact]
        public void Scope_Finds_Symbol()
        {
            var scope1 = new Scope();
            var scope2 = new Scope(scope1);

            var integer = new Type("integer");
            var sym1 = new Symbol("foo", integer);
            var sym2 = new Symbol("bar", integer);

            scope1.Add(sym1);
            scope2.Add(sym2);

            var mustFind1 = scope2.Find(sym1.Name);
            var mustFind2 = scope2.Find(sym2.Name);

            Assert.Equal(sym1, mustFind1);
            Assert.Equal(sym2, mustFind2);
        }
    }
}
