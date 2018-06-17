using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class FeatureSpec : NodeBase
    {
    }

    public class PortSpec : FeatureSpec
    {
        public Token Identifier { get; set; }

        public IoType IoType { get; set; }

        public AssociationBlock Associations { get; set; }

        public PortType PortType { get; set; }

        public override string Type { get; } = nameof(PortSpec);
    }

    public class ParameterSpec : FeatureSpec
    {
        public Token Identifier { get; set; }

        public IoType IoType { get; set; }

        public AssociationBlock Associations { get; set; }

        public ReferenceBase Reference { get; set; }

        public override string Type { get; } = nameof(ParameterSpec);
    }

    public class NoneFeature : FeatureSpec
    {
        public override string Type { get; } = nameof(NoneFeature);
    }
}