using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer
{
    /// <summary>
    /// 封装了调用有限状态机的代码
    /// </summary>
    public static class AutomataTokenizer
    {
        /// <summary>
        /// 使用指定的状态机来遍历源代码字符串，获取语法单元
        /// </summary>
        /// <param name="automataState"></param>
        /// <param name="buffer"></param>
        /// <param name="beginIdx">起始位置</param>
        /// <returns>语法单元的可遍历形式</returns>
        /// <exception cref="TokenizerException"></exception>
        /// <exception cref="WrongTokenException">无法解析当前语法单元时抛出</exception>
        public static IEnumerable<Either<WrongTokenException, Token>> GetAllTokenByAutomata(AutomataTokenizerState automataState, string buffer,
            int beginIdx = 0)
        {
            while (beginIdx < buffer.Length)
            {
                yield return GetByAutomata(automataState, buffer, beginIdx, out beginIdx);
            }
        }

        /// <summary>
        /// 使用有限状态机对源代码进行分割，得到语法单元。
        /// 如果某一个节点能够到达多个匹配当前字符的节点，会按照 <see cref="AutomataTokenizerState.NextStates"/> 中的顺序进行遍历。
        /// </summary>
        /// <param name="automataState">使用的有限状态机</param>
        /// <param name="buffer">要遍历的字符串</param>
        /// <param name="beginIdx">本语法单元的起点</param>
        /// <param name="nextBeginIdx">返回下一个语法单元的起点</param>
        /// <returns>返回读到的最后一个语法单元。如果正常终止，其类型应但是 EndType，否则是不正确的语法</returns>
        /// <exception cref="TokenizerException"></exception>
        /// <exception cref="WrongTokenException">无法解析当前语法单元时抛出</exception>
        public static Either<WrongTokenException, Token> GetByAutomata(AutomataTokenizerState automataState, string buffer, int beginIdx,
            out int nextBeginIdx)
        {
            if (automataState.StateType != AutomataTokenizerStateType.BeginState)
            {
                throw new TokenizerException($"{nameof(automataState)} 必须是起始状态");
            }

            nextBeginIdx = WalkBuffer(buffer, beginIdx, automataState, out var endState);

            if (endState.StateType == AutomataTokenizerStateType.EndState)
            {
                return new Token(buffer.Substring(beginIdx, nextBeginIdx - beginIdx),
                    endState.TokenType, beginIdx, nextBeginIdx);
            }

            // 没有到达终止节点
            if (nextBeginIdx >= buffer.Length)
            {
                return new WrongTokenException("无法识别不完整的源代码", buffer, beginIdx, nextBeginIdx, endState);
            }
            else
            {
                return new WrongTokenException($"在 {nextBeginIdx} 处有无法识别的字符 {buffer[nextBeginIdx]}", buffer, beginIdx, nextBeginIdx, endState);
            }
        }

        /// <summary>
        /// 实际遍历节点
        /// </summary>
        /// <param name="buffer">要遍历的源代码字符串</param>
        /// <param name="beginIdx">字符串起始位置</param>
        /// <param name="state">要遍历的FSM</param>
        /// <param name="endState">(out) 状态机最后一个正确识别的节点</param>
        /// <returns>语法单元的终止位置的下一个位置。</returns>
        /// <example>
        /// 假如遍历下面的字符串，找到单词abc：
        /// abc def
        /// ^  ^
        /// |  |
        /// |  ----------
        /// |           |
        /// beginIdx    returned
        /// </example>
        private static int WalkBuffer(string buffer, int beginIdx, AutomataTokenizerState state,
            out AutomataTokenizerState endState)
        {
            endState = state;

            // 读到结尾
            if (beginIdx >= buffer.Length)
            {
                return buffer.Length;
            }

            foreach (var nextState in state.NextStates)
            {
                if (nextState.Asserter(buffer[beginIdx]))
                {
                    return WalkBuffer(buffer, beginIdx + 1, nextState, out endState);
                }
            }

            // 没有找到合适的下个节点
            return beginIdx;
        }
    }
}