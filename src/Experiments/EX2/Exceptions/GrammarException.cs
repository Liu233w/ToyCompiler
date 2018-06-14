using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.EX2.Libs;
using Liu233w.Compiler.EX2.Nodes;

namespace Liu233w.Compiler.EX2.Exceptions
{
    /// <summary>
    /// 语法错误
    /// </summary>
    public class GrammarException : ParseException
    {
        public GrammarException(string message, Token token, NodeBase errorNode) : base(message, token, errorNode)
        {
        }
    }
}