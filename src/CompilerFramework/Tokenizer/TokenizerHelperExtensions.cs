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

        /// <summary>
        /// 根据 typeTable 转换 Token 类型。用于从标识符中分离出关键字
        /// </summary>
        /// <param name="tokens">输入序列</param>
        /// <param name="sourceTokenType">输入的token序列中需要转换的token类型。如果不匹配，会直接返回此 token</param>
        /// <param name="typeTable">关键词列表。如果 Token.Lexeme 匹配列表中的词，会返回一个 TokenType 是 Lexeme 的 Token</param>
        /// <param name="unMatchedType">如果不匹配关键词列表，返回的 Token 的 TokenType</param>
        /// <returns></returns>
        public static IEnumerable<Token> TransformTokenTypeMatched(this IEnumerable<Token> tokens,
            string sourceTokenType, ISet<string> typeTable, string unMatchedType)
        {
            return tokens.Select(token =>
            {
                if (token.TokenType == sourceTokenType)
                {
                    return new Token(token.Lexeme,
                        typeTable.Contains(token.Lexeme) ? token.Lexeme : unMatchedType,
                        token.TokenBeginIdx, token.TokenEndIdx);
                }
                else
                {
                    return token;
                }
            });
        }
    }
}