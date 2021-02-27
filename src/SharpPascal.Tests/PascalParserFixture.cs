using System;

namespace SharpPascal.Tests
{
    public class PascalParserFixture : IDisposable
    {
        public PascalParser Parser { get; } = new PascalParser();

        public void Dispose() { }
    }
}
