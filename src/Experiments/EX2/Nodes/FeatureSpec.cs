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
    }

    public class ParameterSpec : FeatureSpec
    {
        public Token Identifier { get; set; }

        public IoType IoType { get; set; }

        public AssociationBlock Associations { get; set; }

        public ReferenceBase Reference { get; set; }
    }

    public class NoneFeature : FeatureSpec
    {
    }
}