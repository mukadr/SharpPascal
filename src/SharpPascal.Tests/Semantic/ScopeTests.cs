using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Semantic;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class ScopeTests
    {
        [TestMethod]
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

            Assert.AreEqual(sym1, mustFind1);
            Assert.IsNull(mustFail1);

            var mustFind2 = scope2.FindLocal(sym2.Name);
            var mustFail2 = scope2.FindLocal(sym1.Name);

            Assert.AreEqual(sym2, mustFind2);
            Assert.IsNull(mustFail2);
        }

        [TestMethod]
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

            Assert.AreEqual(sym1, mustFind1);
            Assert.AreEqual(sym2, mustFind2);
        }
    }
}
