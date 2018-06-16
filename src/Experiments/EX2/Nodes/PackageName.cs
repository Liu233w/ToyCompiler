using System.Collections.Generic;
using Liu233w.Compiler.CompilerFramework.Tokenizer;

namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class PackageNameBase : NodeBase
    {
    }

    public class PackageName : PackageNameBase
    {
        public ICollection<Token> Identifiers { get; set; }
    }

    public class NonePackageName : PackageNameBase
    {
    }
}