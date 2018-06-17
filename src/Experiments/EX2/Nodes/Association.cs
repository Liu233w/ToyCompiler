using System.Collections.Generic;
using System.Runtime.Serialization;
using LanguageExt;
using LanguageExt.TypeClasses;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Newtonsoft.Json;

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

        public override string Type { get; } = nameof(Association);
    }

    public class NoneAssociation : AssociationBase
    {
        public override string Type { get; } = nameof(NoneAssociation);
    }

    public enum Splitter
    {
        /// <summary>
        ///     =>
        /// </summary>
        [EnumMember(Value = "=>")] Arrow,

        /// <summary>
        ///     +=>
        /// </summary>
        [EnumMember(Value = "+=>")] PlusArrow
    }

    public class AssociationBlock : NodeBase
    {
        public ICollection<AssociationBase> Associations { get; set; }

        public override string Type { get; } = nameof(AssociationBlock);
    }
}