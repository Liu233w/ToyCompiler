namespace Liu233w.Compiler.EX2.Nodes
{
    public abstract class NodeBase
    {
        /// <summary>
        ///     语法树的起始位置
        /// </summary>
        public int BeginPosition { get; set; }

        /// <summary>
        ///     语法树的终止位置
        /// </summary>
        public int EndPosition { get; set; }
    }
}