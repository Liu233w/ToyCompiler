using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public class PackageName : NodeBase
    {
        public Token Identifier { get; set; }

        public PackageName Parent { get; set; }
    }
}