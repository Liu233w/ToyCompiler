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

        private BnfDefination _bnfDefinationWithEpsilon;

        public DfsParserTest()
        {
            _defination = new BnfDefination()
                    .AddRule("S", new[] { "a", "A", TerminalConsts.EndOfFile })
                    .AddRule("S", new[] { "c", "A", "d", TerminalConsts.EndOfFile })
                    .AddRule("A", new[] { "a" })
                    .AddRule("A", new[] { "a", "b" })
                ;

            _bnfDefinationWithEpsilon = new BnfDefination()
                    .AddRule("A", new[] { "0", "B", TerminalConsts.EndOfFile })
                    .AddRule("B", new[] { "1", "B" })
                    .AddRule("B", TerminalConsts.Epsilon)
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
                }, new[] {"c", "A", "d", TerminalConsts.EndOfFile}),
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
                }, new[] {"a", "A", TerminalConsts.EndOfFile}),
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
                }, new[] {"a", "A", TerminalConsts.EndOfFile}),
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
                }, new[] {"c", "A", "d", TerminalConsts.EndOfFile}),

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

        [Theory]
        [MemberData(nameof(GetEpislonAssertions))]
        public void 能够检测带ɛ的句子(string input, NonTerminalTree expected)
        {
            var tokens = PrepareTokens(input);
            var result = DfsParser.Parse(tokens, _defination, "A");
            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> GetEpislonAssertions()
        {
            yield return new object[]
            {
                "0",
                new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("0", "0", 0,1)),
                    new NonTerminalTree("B", new ISyntacticAnalysisTree[]
                    {
                        null
                    }, TerminalConsts.Epsilon),
                }, new[] {"0", "B"}),
            };

            yield return new object[]
            {
                "01",
                new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                {
                    new TerminalTree(new Token("0", "0", 0, 1)),
                    new NonTerminalTree("B", new ISyntacticAnalysisTree[]
                    {
                        new TerminalTree(new Token("1", "1", 1, 2)),
                        new NonTerminalTree("A", new ISyntacticAnalysisTree[]
                        {
                            new TerminalTree(new Token("0", "0", 2, 3)),
                            new NonTerminalTree("B", new ISyntacticAnalysisTree[]
                            {
                                null
                            }, TerminalConsts.Epsilon),
                        }, new[] {"0", "B"}),
                    }, new[] {"1", "A"}),
                }, new[] {"0", "B"}),
            };

            yield return new object[]
            {
                "00", null
            };

            yield return new object[]
            {
                "1", null
            };
        }
    }
}