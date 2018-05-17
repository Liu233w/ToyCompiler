using System;
using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.CompilerFramework.Parser.TreeNodes
{
    /// <summary>
    /// 表示一个终结符的节点，直接用 token 来表示终结符
    /// </summary>
    public class TerminalTree : ISyntacticAnalysisTree, IEquatable<TerminalTree>
    {
        public Token Token { get; }

        public TerminalTree(Token token)
        {
            Token = token;
        }

        public string GetName() => Token.TokenType;

        public int GetBeginPosition() => Token.TokenBeginIdx;

        public int GetEndPosition() => Token.TokenEndIdx;

        public bool Equals(ISyntacticAnalysisTree other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other as TerminalTree);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as TerminalTree);
        }

        public override int GetHashCode()
        {
            return (Token != null ? Token.GetHashCode() : 0);
        }

        public bool Equals(TerminalTree other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Token.Equals(other.Token);
        }
    }
}