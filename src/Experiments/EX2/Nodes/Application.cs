using System.Collections.Generic;

namespace Liu233w.Compiler.EX2.Nodes
{
    public class Application : NodeBase
    {
        public ICollection<ThreadSpec> Threads { get; set; }
    }
}