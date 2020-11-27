﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static SharpPascal.Analyzer;
using static SharpPascal.PascalParser;

namespace SharpPascal.Tests
{
    [TestClass]
    public class AnalyzerTests
    {
        [TestMethod]
        public void Analyzer_Complains_About_Redeclared_Variable()
        {
            const string source = @"
                var
                    x: Integer;
                    x: Integer;

                begin end.
            ";

            var unit = Parse(source);

            var diagnostics = TypeCheck(unit);

            var redeclaredVariableError = diagnostics.First();

            Assert.IsTrue(redeclaredVariableError.IsError);
            Assert.IsTrue(redeclaredVariableError.Message.Contains("redeclared"));
        }

        [TestMethod]
        public void Analyzer_Complains_About_Unknown_Type()
        {
            const string source = @"
                var
                    x: Foobar;
                    y: Integer;

                begin end.
            ";

            var unit = Parse(source);

            var diagnostics = TypeCheck(unit);

            var unknownTypeError = diagnostics.First();

            Assert.IsTrue(unknownTypeError.IsError);
            Assert.IsTrue(unknownTypeError.Message.Contains("type"));

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    if (var.Name == "x")
                    {
                        Assert.AreEqual(Type.Unknown, var.Type);
                    }
                    else
                    {
                        Assert.AreEqual(Type.Integer, var.Type);
                    }
                }
            });
        }

        [TestMethod]
        public void Analyzer_Checks_Variable_Declaration()
        {
            const string source = @"
                var
                    x: Integer;

                begin end.
            ";

            var unit = Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.AreEqual(0, diagnostics.Count());

            unit.Visit(new Visitor
            {
                VisitVarDeclaration = var =>
                {
                    Assert.AreEqual(Type.Integer, var.Type);
                }
            });
        }

        [TestMethod]
        public void Analyzer_Checks_Arithmetic_Expression()
        {
            const string source = @"
                var
                    x: Integer;

                begin
                    x := 1 * (5 - x);
                end.
            ";

            var unit = Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.AreEqual(0, diagnostics.Count());
        }

        public void Analyzer_Checks_Comparison_Expression()
        {
            const string source = @"
                var
                    x: Boolean;

                begin
                    x := (10 > 5) = (5 > 10);
                end.
            ";

            var unit = Parse(source);

            var diagnostics = TypeCheck(unit);

            Assert.AreEqual(0, diagnostics.Count());
        }
    }
}