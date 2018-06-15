using System;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Exceptions
{
    /// <summary>
    /// Token 数量不够时抛出的异常
    /// </summary>
    public class NotEnoughTokenException : ParseException
    {
        public NotEnoughTokenException(string message, Token token, NodeBase errorNode) : base(message, token, errorNode)
        {
        }
    }
}