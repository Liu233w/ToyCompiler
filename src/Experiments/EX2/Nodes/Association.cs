using System.Collections.Generic;
using LanguageExt;
using LanguageExt.TypeClasses;
using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class AssociationBase : NodeBase
    {
    }

    public class Association : AssociationBase
    {
        public Option<Token> PreIdentifier { get; set; }

        public Token Identifier { get; set; }

        public Splitter Splitter { get; set; }

        public bool Constant { get; set; }

        public Token Decimal { get; set; }
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

    public class AssociationBlock : NodeBase
    {
        public ICollection<AssociationBase> Associations { get; set; }
    }
}