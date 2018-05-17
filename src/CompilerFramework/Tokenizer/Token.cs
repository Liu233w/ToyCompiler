using System;
using System.Diagnostics;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer
{
    /// <summary>
    /// 表示一个语法单元的数据结构
    /// </summary>
    [DebuggerDisplay("Lexeme {Lexeme} TokenType {TokenType} {TokenBeginIdx}-{TokenEndIdx}")]
    public class Token : IEquatable<Token>
    {
        /// <summary>
        /// 语法单元的词素
        /// </summary>
        public string Lexeme { get; }

        /// <summary>
        /// 语法单元的类型
        /// </summary>
        public string TokenType { get; }

        /// <summary>
        /// 语法单元的第一个字符在源代码中的位置
        /// </summary>
        public int TokenBeginIdx { get; }

        /// <summary>
        /// 语法单元的最后一个字符的下个字符在源代码中的位置。
        /// </summary>
        public int TokenEndIdx { get; }

        public Token(string lexeme, string tokenType, int tokenBeginIdx, int tokenEndIdx)
        {
            Lexeme = lexeme;
            TokenType = tokenType;
            TokenBeginIdx = tokenBeginIdx;
            TokenEndIdx = tokenEndIdx;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Token);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Lexeme != null ? Lexeme.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TokenType != null ? TokenType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TokenBeginIdx;
                hashCode = (hashCode * 397) ^ TokenEndIdx;
                return hashCode;
            }
        }

        public bool Equals(Token other)
        {
            return other != null &&
                   Lexeme == other.Lexeme &&
                   TokenType == other.TokenType &&
                   TokenBeginIdx == other.TokenBeginIdx &&
                   TokenEndIdx == other.TokenEndIdx;
        }
    }
}