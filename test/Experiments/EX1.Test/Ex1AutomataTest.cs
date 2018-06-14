using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Test;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX1.Libs;
using Shouldly;
using Xunit;

namespace Liu233w.Compiler.EX1.Test
{
    public class Ex1AutomataTest
    {
        [Theory]
        [InlineData("name", 4)]
        [InlineData("name1", 5)]
        [InlineData("n_me1", 5)]
        [InlineData("n_me", 4)]
        [InlineData("n", 1)]
        [InlineData("n__", 3)]
        [InlineData("n23", 3)]
        [InlineData("n2_3", 4)]
        [InlineData("z@", 1)]
        public void 自动机_能够识别Symbol(string buffer, int expectEndAt)
        {
            var autoMata = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState> { Ex1Automata.Symbol });

            var res = AutomataTokenizer.GetByAutomata(autoMata, buffer, 0, out var endIdx);
            res.ShouldBeRight(token => token.TokenType.ShouldBe(Ex1Automata.SymbolToken));
            endIdx.ShouldBe(expectEndAt);
        }

        [Theory]
        [InlineData("_", 0)]
        [InlineData("_a", 0)]
        [InlineData("_1", 0)]
        [InlineData("123", 0)]
        [InlineData("@", 0)]
        public void 自动机_无法识别Symbol时能够报错(string buffer, int expectCurrentIdx)
        {
            var autoMata = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState> { Ex1Automata.Symbol });

            AutomataTokenizer.GetByAutomata(autoMata, buffer, 0, out _)
                .ShouldBeLeft(exception => { exception.CurrentIdx.ShouldBe(expectCurrentIdx); });
        }

        [Theory]
        [InlineData("1.1", 3, TokenTypes.Decimal)]
        [InlineData("+1.1", 4, TokenTypes.Decimal)]
        [InlineData("-1.1", 4, TokenTypes.Decimal)]
        [InlineData("123.1", 5, TokenTypes.Decimal)]
        [InlineData("1.231", 5, TokenTypes.Decimal)]
        [InlineData("11.11", 5, TokenTypes.Decimal)]
        [InlineData("-11.11", 6, TokenTypes.Decimal)]
        [InlineData("-50.0", 5, TokenTypes.Decimal)]
        [InlineData("+=>aaa", 3, TokenTypes.Arraw2)]
        [InlineData("->aaa", 2, TokenTypes.Arraw3)]
        public void 自动机_能够识别Decimal_Arrow2_Arrow3(string buffer, int expectEndAt, string expectedTokenType)
        {
            var autoMata = AutomataTokenizerState.ForBegin(Ex1Automata.DecimalOrArrow2OrArrow3);

            var res = AutomataTokenizer.GetByAutomata(autoMata, buffer, 0, out var endIdx);
            res.ShouldBeRight(token => token.TokenType.ShouldBe(expectedTokenType));
            endIdx.ShouldBe(expectEndAt);
        }

        [Theory]
        [InlineData("a", 0)]
        [InlineData("+.1", 1)]
        [InlineData("+=", 2)]
        [InlineData("+1.", 3)]
        [InlineData("+1.=>", 3)]
        [InlineData("-", 1)]
        [InlineData("-1.", 3)]
        [InlineData("-.11", 1)]
        public void 自动机_在无法识别Decimal_Arrow2_Arrow3时能够报错(string buffer, int expectCurrentIdx)
        {
            var autoMata = AutomataTokenizerState.ForBegin(Ex1Automata.DecimalOrArrow2OrArrow3);

            AutomataTokenizer.GetByAutomata(autoMata, buffer, 0, out _)
                .ShouldBeLeft(exception => { exception.CurrentIdx.ShouldBe(expectCurrentIdx); });
        }
    }
}