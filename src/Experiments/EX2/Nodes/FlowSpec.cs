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

        public override string Type { get; } = nameof(FlowSourceSpec);
    }

    public class FlowSinkSpec : FlowSpec
    {
        public Token PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public AssociationBlock Associations { get; set; }

        public override string Type { get; } = nameof(FlowSinkSpec);
    }

    public class FlowPathSpec : FlowSpec
    {
        public Token PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public Token DestIdentifier { get; set; }

        public override string Type { get; } = nameof(FlowPathSpec);
    }

    public class NoneFlowSpec : FlowSpec
    {
        public override string Type { get; } = nameof(NoneFlowSpec);
    }
}