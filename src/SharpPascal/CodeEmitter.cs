using System.Text;

namespace SharpPascal
{
    public sealed class CodeEmitter
    {
        private readonly StringBuilder _code;

        public CodeEmitter()
        {
            _code = new StringBuilder();
        }

        public void Emit(string code)
        {
            _code.AppendLine(code);
        }

        public override string ToString() => _code.ToString();
    }
}