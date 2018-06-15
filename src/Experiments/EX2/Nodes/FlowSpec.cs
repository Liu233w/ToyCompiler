using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class FlowSpec : NodeBase
    {
    }

    public class FlowSourceSpec : FlowSpec
    {
        public Token PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public AssociationBlock Associations { get; set; }
    }

    public class FlowSinkSpec : FlowSpec
    {
        public Token PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public AssociationBlock Associations { get; set; }
    }

    public class FlowPathSpec : FlowSpec
    {
        public Token PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public Token DestIdentifier { get; set; }
    }
}