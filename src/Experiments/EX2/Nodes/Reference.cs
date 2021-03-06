﻿using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class ReferenceBase : NodeBase
    {
    }

    public class Reference : ReferenceBase
    {
        public PackageNameBase PackageName { get; set; }
        
        public Token Identifier { get; set; }

        public override string @Type { get; } = nameof(Reference);
    }

    public class NoneReference : ReferenceBase
    {
        public override string @Type { get; } = nameof(NoneReference);
    }
}