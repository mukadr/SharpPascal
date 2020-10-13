using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Binding;
using SharpPascal.Syntax;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class BindingTests
    {
        [TestMethod]
        public void Binder_Declares_Variables()
        {
            var source = @"
                var
                    x: integer;
                    y: integer;
                    z: integer;

                begin end.
            ";

            var tree = PascalParser.Parse(source);

            var boundUnit = Binder.SyntaxCheck(tree);

            var x = boundUnit.Scope.Find("x");
            var y = boundUnit.Scope.Find("y");
            var z = boundUnit.Scope.Find("z");

            Assert.IsNotNull(x);
            Assert.IsNotNull(y);
            Assert.IsNotNull(z);

            Assert.AreEqual("integer", x!.Type.Name);
            Assert.AreEqual("integer", y!.Type.Name);
            Assert.AreEqual("integer", z!.Type.Name);
        }
    }
}
