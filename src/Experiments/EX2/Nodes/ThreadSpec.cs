using System;

namespace Liu233w.Compiler.EX2.Nodes
{
    public class ThreadSpec
    {
        public string Identifier { get; set; }

        public FeatureSpec Features { get; set; }

        public FlowSpec Flows { get; set; }

        public AssociationBase Properties { get; set; }
    }
}