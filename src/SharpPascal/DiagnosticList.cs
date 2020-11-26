using SharpPascal.Parsing;
using System.Collections;
using System.Collections.Generic;

namespace SharpPascal
{
    public class DiagnosticList : IEnumerable<Diagnostic>
    {
        private readonly IList<Diagnostic> _diagnostics;

        public DiagnosticList()
        {
            _diagnostics = new List<Diagnostic>();
        }

        public void AddError(string message, Location location)
        {
            _diagnostics.Add(Diagnostic.Error(message, location));
        }

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
