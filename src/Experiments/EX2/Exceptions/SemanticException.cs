using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Exceptions
{
    /// <summary>
    /// 语义错误
    /// </summary>
    public class SemanticException : ParseException
    {
        public SemanticException(string message, Token token, NodeBase errorNode) : base(message, token, errorNode)
        {
        }
    }
}