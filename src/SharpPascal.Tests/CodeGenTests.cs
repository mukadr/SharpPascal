using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static SharpPascal.Analyzer;
using static SharpPascal.PascalParser;

namespace SharpPascal.Tests
{
    [TestClass]
    public class CodeGenTests
    {
        [TestMethod]
        public void Generates_Code_For_Empty_Program()
        {
            const string source = @"begin end.";

            var unit = Parse(source);
            Assert.AreEqual(0, TypeCheck(unit).Count());

            var emitter = new CodeEmitter();
            unit.Emit(emitter);

            Assert.AreEqual(@".global main
main:
  push {fp, lr}
  mov r0, #0
  pop {fp, pc}
", emitter.ToString());
        }

        [TestMethod]
        public void Generates_Code_For_Simple_Assignment()
        {
            const string source = @"
                var
                    x: integer;
                begin
                    x := 658;
                end.";

            var unit = Parse(source);
            Assert.AreEqual(0, TypeCheck(unit).Count());

            var emitter = new CodeEmitter();
            unit.Emit(emitter);

            // FIXME: str
            Assert.AreEqual(@".global main
main:
  push {fp, lr}
  ldr r0, #658
  mov r0, #0
  pop {fp, pc}
", emitter.ToString());
        }
    }
}
