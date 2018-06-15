using System.Collections.Generic;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class AssociationBase : NodeBase
    {
    }

    public class Association : AssociationBase
    {
        public string PreIdentifier { get; set; }

        public string Identifier { get; set; }

        public Splitter Splitter { get; set; }

        public bool Constant { get; set; }

        public string Decimal { get; set; }
    }

    public class NoneAssociation : AssociationBase
    {
    }

    public enum Splitter
    {
        /// <summary>
        ///     =>
        /// </summary>
        Arrow,

        /// <summary>
        ///     +=>
        /// </summary>
        PlusArrow
    }

    public class AssociationBlock
    {
        public ICollection<AssociationBase> Associations { get; set; }
    }
}