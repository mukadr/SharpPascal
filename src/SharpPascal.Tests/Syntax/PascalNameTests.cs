using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpPascal.Syntax;

namespace SharpPascal.Tests.Syntax
{
    [TestClass]
    public class PascalNameTests
    {
        [TestMethod]
        public void PascalName_Equals_CompareCaseInsensitive()
        {
            Assert.AreEqual(new PascalName("foobar"), new PascalName("FOOBAR"));
        }

        [TestMethod]
        public void PascalName_Overloads_OperatorEquals()
        {
            Assert.IsTrue(new PascalName("foobar") == new PascalName("FOOBAR"));
            Assert.IsTrue(new PascalName("foobar") == "FOOBAR");
            Assert.IsTrue("FOOBAR" == new PascalName("foobar"));
        }
    }
}
