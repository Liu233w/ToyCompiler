using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public class ThreadSpec : NodeBase
    {
        public Token Identifier { get; set; }

        public FeatureSpec Features { get; set; }

        public FlowSpec Flows { get; set; }

        public AssociationBase Properties { get; set; }
    }
}