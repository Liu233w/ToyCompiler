using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class ReferenceBase : NodeBase
    {
    }

    public class Reference : ReferenceBase
    {
        public PackageName PackageName { get; set; }
        
        public Token Identifier { get; set; }
    }

    public class NoneReference : ReferenceBase
    {
    }
}