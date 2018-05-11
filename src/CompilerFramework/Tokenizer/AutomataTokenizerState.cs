using System;
using System.Collections.Generic;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer
{
    /// <summary>
    /// 表示有限状态机中的一个节点，用于 <see cref="AutomataTokenizer"/>
    /// </summary>
    public class AutomataTokenizerState
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public AutomataTokenizerStateType StateType { get; private set; }

        /// <summary>
        /// 表示到达此状态需要的验证。
        /// </summary>
        public Func<char, bool> Asserter { get; private set; }

        /// <summary>
        /// 本节点能够到达的其他节点列表
        /// </summary>
        public IList<AutomataTokenizerState> NextStates { get; private set; }

        /// <summary>
        /// 到达终止状态时识别的类型。
        /// </summary>
        public string TokenType { get; private set; }

        private AutomataTokenizerState()
        {
        }

        /// <summary>
        /// 生成一个初始节点
        /// </summary>
        /// <param name="nextStates"><see cref="NextStates"/></param>
        /// <returns></returns>
        public static AutomataTokenizerState ForBegin(IList<AutomataTokenizerState> nextStates)
        {
            return new AutomataTokenizerState
            {
                StateType = AutomataTokenizerStateType.BeginState,
                NextStates = nextStates,
            };
        }

        /// <summary>
        /// 生成一个终止节点（终止节点可以到达一个新的节点）
        /// </summary>
        /// <param name="asserter"><see cref="Asserter"/></param>
        /// <param name="tokenType"><see cref="TokenType"/></param>
        /// <param name="nextStates"><see cref="NextStates"/>；不写时表示不会去新的节点</param>
        /// <returns></returns>
        public static AutomataTokenizerState ForEnd(Func<char, bool> asserter, string tokenType,
            IList<AutomataTokenizerState> nextStates = null)
        {
            return new AutomataTokenizerState
            {
                StateType = AutomataTokenizerStateType.EndState,
                Asserter = asserter,
                TokenType = tokenType,
                NextStates = nextStates ?? new List<AutomataTokenizerState>(),
            };
        }

        public static AutomataTokenizerState ForMiddle(Func<char, bool> asserter,
            IList<AutomataTokenizerState> nextStates)
        {
            return new AutomataTokenizerState
            {
                StateType = AutomataTokenizerStateType.MiddleState,
                Asserter = asserter,
                NextStates = nextStates,
            };
        }
    }

    /// <summary>
    /// 表示状态机节点的类型
    /// </summary>
    public enum AutomataTokenizerStateType
    {
        /// <summary>
        /// 开始状态
        /// </summary>
        BeginState,
        /// <summary>
        /// 终止状态
        /// </summary>
        EndState,
        /// <summary>
        /// 中间状态
        /// </summary>
        MiddleState,
    }
}