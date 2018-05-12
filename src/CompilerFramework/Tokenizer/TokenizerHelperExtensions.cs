using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;

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
            return it => it == c;
        }

        /// <summary>
        /// 从词法单元流中移除指定类型的Token
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public static IEnumerable<Token> ExcludeTokenType(this IEnumerable<Token> tokens, string tokenType)
        {
            return tokens.Where(token => token.TokenType != tokenType);
        }

        /// <summary>
        /// 将 Token 流分成两部分，分别表示无法识别的语法单元和能够识别的语法单元
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Either<List<WrongTokenException>, List<Token>> Seprate(
            this IEnumerable<Either<WrongTokenException, Token>> self)
        {
            var enumerable = self.ToList();

            var errors = enumerable.Lefts().ToList();
            if (errors.Count > 0)
            {
                return errors;
            }
            else
            {
                return enumerable.Rights().ToList();
            }
        }
    }
}