using System;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer
{
    /// <summary>
    /// 方便使用 Tokenizer 的扩展
    /// </summary>
    public static class TokenizerHelperExtensions
    {
        /// <summary>
        /// 便于使用 <see cref="AutomataTokenizerState"/> 编写规则。返回一个判断当前字符是否等于给定字符的函数。
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Func<char, bool> MatchCurrentPosition(this char c)
        {
            return it => it == 'c';
        }
    }
}