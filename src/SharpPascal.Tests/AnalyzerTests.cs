using System.Linq;
using Xunit;
using static SharpPascal.Analyzer;

namespace SharpPascal.Tests
{
    public class AnalyzerTests : IClassFixture<PascalParserFixture>
    {
        private readonly PascalParser _parser;

        public AnalyzerTests(PascalParserFixture pascalParserFixture)
        {
            _parser = pascalParserFixture.Parser;
        }

        [Fact]
        public void Analyzer_Complains_About_Redeclared_Variable()
        {
            const string source = @"
                var
                    x: Integer;
                    x: Integer;

                begin end.
            ";

            var unit = _parser.Parse(source);

            var diagnostics = TypeCheck(unit);

            var redeclaredVariableError = diagnostics.First();

            Assert.True(redeclaredVariableError.IsError);
            Assert.Contains("redeclared", redeclaredVariableError.Message);
        }

        [Fact]
        public void Analyzer_Complains_About_Unknown_Type()
        {
            const string source = @"
                var
                    x: Foobar;
                    y: Integer;

                begin end.
            ";

            var unit = _parser.Parse(source);

            var diagnostics = TypeCheck(unit);

            var unknownTypeError = diagnostics.First();

            Assert.True(unknownTypeError.IsError);
            Assert.Contains("type", unknownTypeError.Message);

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    if (var.Name == "x")
                    {
                        Assert.Equal(Type.Unknown, var.Type);
                    }
                    else
                    {
                        Assert.Equal(Type.Integer, var.Type);
                    }
                }
            });
        }

        [Fact]
        public void Analyzer_Checks_Variable_Declaration()
        {
            const string source = @"
                var
                    x: Integer;

                begin end.
            ";

            var unit = _parser.Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.Empty(diagnostics);

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    Assert.Equal(Type.Integer, var.Type);
                }
            });
        }

        [Fact]
        public void Analyzer_Checks_Arithmetic_Expression()
        {
            const string source = @"
                var
                    x: Integer;

                begin
                    x := 1 * (5 - x);
                end.
            ";

            var unit = _parser.Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.Empty(diagnostics);
        }

        [Fact]
        public void Analyzer_Checks_Comparison_Expression()
        {
            const string source = @"
                var
                    x: Boolean;

                begin
                    x := (10 > 5) = (5 > 10);
                end.
            ";

            var unit = _parser.Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.Empty(diagnostics);
        }
    }
}
