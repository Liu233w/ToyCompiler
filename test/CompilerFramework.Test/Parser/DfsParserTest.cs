using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Liu233w.Compiler.CompilerFramework.Parser;
using Liu233w.Compiler.CompilerFramework.Parser.TreeNodes;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Xunit;

namespace Liu233w.Compiler.CompilerFramework.Test.Parser
{
    public class DfsParserTest
    {
        private BnfDefination _defination;

        public DfsParserTest()
        {
            _defination = new BnfDefination()
                    .AddRule("S", new[] { "a", "A", null })
                    .AddRule("S", new[] { "c", "A", "d", null })
                    .AddRule("A", new[] { "a" })
                    .AddRule("A", new[] { "a", "b" })
                ;
        }

        [Theory]
        [MemberData(nameof(GetCorrectCombinationsForAnalyze))]
        public void 在输入正确时能够得到结果(string input, NonTerminalTree expect)
        {
            var tokens = PrepareTokens(input);
            var result = DfsParser.Parse(tokens, _defination, "S");
            result.Should().BeEquivalentTo(expect);
        }

        private static List<Token> PrepareTokens(string str)
        {
            return str
                .Select((c, i) => new Token(c.ToString(), c.ToString(), i, i + 1))
                .ToList();
        }

        public static IEnumerable<object[]> GetCorrectCombinationsForAnalyze()
        {
            yield return new object[]
            {
                "cabd",
                new NonTerminalTree("S", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("c", "c", 0, 1)),
                    new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                    {
                        new TerminalTree(new Token("a", "a", 1, 2)),
                        new TerminalTree(new Token("b", "b", 2, 3)),
                    }, new[] {"a", "b"}),
                    new TerminalTree(new Token("d", "d", 3, 4)),
                    null,
                }, new[] {"c", "A", "d", null}),
            };

            yield return new object[]
            {
                "aa",
                new NonTerminalTree("S", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("a", "a", 0, 1)),
                    new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                    {
                        new TerminalTree(new Token("a", "a", 1, 2)),
                    }, new[] {"a"}),
                    null,
                }, new[] {"a", "A", null}),
            };

            yield return new object[]
            {
                "aab",
                new NonTerminalTree("S", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("a", "a", 0, 1)),
                    new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                    {
                        new TerminalTree(new Token("a", "a", 1, 2)),
                        new TerminalTree(new Token("b", "b", 2, 3)),
                    }, new[] {"a", "b"}),
                    null,
                }, new[] {"a", "A", null}),
            };

            yield return new object[]
            {
                "cad",
                new NonTerminalTree("S", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("c", "c", 0, 1)),
                    new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                    {
                        new TerminalTree(new Token("a", "a", 1, 2)),
                    }, new[] {"a"}),
                    new TerminalTree(new Token("d", "d", 2, 3)),
                    null,
                }, new[] {"c", "A", "d", null}),

            };
        }

        [Theory]
        [InlineData("zzz")]
        [InlineData("ab")]
        [InlineData("abd")]
        [InlineData("cd")]
        [InlineData("")]
        [InlineData("aa ")]
        public void 在输入不正确的句子时能够检测到错误(string input)
        {
            var tokens = PrepareTokens(input);
            var result = DfsParser.Parse(tokens, _defination, "S");
            result.Should().Be(null);
        }
    }
}