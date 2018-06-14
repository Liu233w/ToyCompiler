using System;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Exceptions
{
    /// <summary>
    /// 语法分析器抛出的异常
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// 出错时的Token
        /// </summary>
        public Token Token { get; set; }
        
        /// <summary>
        /// 找到了错误的语法树节点
        /// </summary>
        public NodeBase ErrorNode { get; set; }

        public ParseException(string message, Token token, NodeBase errorNode) : base(message)
        {
            Token = token;
            ErrorNode = errorNode;
        }
    }
}