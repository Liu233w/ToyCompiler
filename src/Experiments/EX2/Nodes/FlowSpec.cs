namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class FlowSpec
    {
    }

    public class FlowSourceSpec : FlowSpec
    {
        public string PreIdentifier { get; set; }

        public string Identifier { get; set; }

        public AssociationBlock Associations { get; set; }
    }

    public class FlowSinkSpec : FlowSpec
    {
        public string PreIdentifier { get; set; }

        public string Identifier { get; set; }

        public AssociationBlock Associations { get; set; }
    }

    public class FlowPathSpec : FlowSpec
    {
        public string PreIdentifier { get; set; }

        public string Identifier { get; set; }

        public string DestIdentifier { get; set; }
    }
}