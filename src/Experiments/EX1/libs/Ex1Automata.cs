using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX1.Libs
{
    /// <summary>
    /// 用于Ex1的词法分析器
    /// </summary>
    public static class Ex1Automata
    {
        public static AutomataTokenizerState LeftBrace =>
            AutomataTokenizerState.ForEnd('{'.MatchCurrentPosition(), TokenTypes.LeftBrace);

        public static AutomataTokenizerState RightBrace =>
            AutomataTokenizerState.ForEnd('}'.MatchCurrentPosition(), TokenTypes.RightBrace);

        public static AutomataTokenizerState SemiColon =>
            AutomataTokenizerState.ForEnd(';'.MatchCurrentPosition(), TokenTypes.Semicolon);

        public static AutomataTokenizerState Colons =>
            AutomataTokenizerState.ForEnd(':'.MatchCurrentPosition(), TokenTypes.SingleColon,
                new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForEnd(':'.MatchCurrentPosition(), TokenTypes.DoubleColon),
                });

        public static AutomataTokenizerState Arrow1 =>
            AutomataTokenizerState.ForMiddle('='.MatchCurrentPosition(), new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForEnd('>'.MatchCurrentPosition(), TokenTypes.Arraw1),
            });

        /// <summary>
        ///  识别数字和 Arrow2。有多个入口，所以返回数组
        /// </summary>
        public static AutomataTokenizerState[] DecimalOrArrow2OrArrow3
        {
            get
            {
                var endState = AutomataTokenizerState.ForEnd(char.IsDigit, TokenTypes.Decimal);
                endState.NextStates.Add(endState); // 自循环

                var endStateWithDot = AutomataTokenizerState.ForMiddle('.'.MatchCurrentPosition(),
                    new List<AutomataTokenizerState> { endState });

                var numeral = AutomataTokenizerState.ForMiddle(char.IsDigit,
                    new List<AutomataTokenizerState> { endStateWithDot });
                numeral.NextStates.Add(numeral);

                var arrow3OrNumeral = AutomataTokenizerState.ForMiddle('-'.MatchCurrentPosition(),
                    new List<AutomataTokenizerState>
                    {
                        numeral,
                        AutomataTokenizerState.ForEnd('>'.MatchCurrentPosition(), TokenTypes.Arraw3),
                    });

                var arrow2OrNumeral =
                    AutomataTokenizerState.ForMiddle('+'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                    {
                        AutomataTokenizerState.ForMiddle('='.MatchCurrentPosition(), new List<AutomataTokenizerState>
                        {
                            AutomataTokenizerState.ForEnd('>'.MatchCurrentPosition(), TokenTypes.Arraw2)
                        }),
                        numeral,
                    });

                return new[] { numeral, arrow3OrNumeral, arrow2OrNumeral };
            }
        }

        /// <summary>
        /// 所有的空格符。由于在分析结束后会被抛弃掉，所以在这里声明
        /// </summary>
        public const string SpaceToken = "space";

        /// <summary>
        /// 符号，包含关键字和 identifier，在分析结束后会被转换成那两个语法单元，所以在这里声明
        /// </summary>
        public const string SymbolToken = "symbol";

        public static AutomataTokenizerState Space
        {
            get
            {
                var state = AutomataTokenizerState.ForEnd(c => c == ' ' || c == '\r' || c == '\n' || c == '\t', SpaceToken);
                state.NextStates.Add(state);
                return state;
            }
        }

        public static AutomataTokenizerState Symbol
        {
            get
            {
                var endState = AutomataTokenizerState.ForEnd(c => char.IsLetterOrDigit(c) || c == '_', SymbolToken);
                endState.NextStates.Add(endState);
                return AutomataTokenizerState.ForEnd(char.IsLetter, SymbolToken, new List<AutomataTokenizerState> { endState });
            }
        }

        public static AutomataTokenizerState ForBegin
        {
            get
            {
                var automata = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
                {
                    LeftBrace,
                    RightBrace,
                    Arrow1,
                    Colons,
                    SemiColon,
                    Space,
                    Symbol,
                });
                foreach (var state in DecimalOrArrow2OrArrow3)
                {
                    automata.NextStates.Add(state);
                }

                return automata;
            }
        }
    }
}