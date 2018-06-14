using System.Runtime.InteropServices;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class FeatureSpec
    {
    }

    public class PortSpec : FeatureSpec
    {
        public string Identifier { get; set; }

        public IoType IoType { get; set; }

        public AssociationBlock Associations { get; set; }

        public PortType PortType { get; set; }
    }

    public class ParameterSpec : FeatureSpec
    {
        public string Identifier { get; set; }

        public IoType IoType { get; set; }

        public AssociationBlock Associations { get; set; }

        public Reference Reference { get; set; }
    }

    public class NoneFeature : FeatureSpec
    {
    }
}