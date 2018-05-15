using FluentAssertions;
using Liu233w.Compiler.CodePiece.LexicalAnalyzer;
using Xunit;

namespace Liu233w.Compiler.CodePiece.test.LexicalAnalyzer
{
    public class LlAnalyzerTest
    {
        [Theory]
        [InlineData("(a)a", true)]
        [InlineData("a", true)]
        [InlineData("(a)", false)]
        [InlineData("b", false)]
        [InlineData("(a)b", false)]
        [InlineData("(a", false)]
        [InlineData("(b", false)]
        public void Specific(string input, bool expected)
        {
            LlAnalyzer.Specific(input).Should().Be(expected);
        }
    }
}