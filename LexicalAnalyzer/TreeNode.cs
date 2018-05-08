using System.Collections.Generic;

namespace LexicalAnalyzer
{
    /// <summary>
    /// 一个语法树结点（可能是分支节点也可能是叶子节点）
    /// </summary>
    public abstract class Tree
    {
        public char Token { get; set; }
    }

    /// <summary>
    /// 分支节点，代表非终止符号
    /// </summary>
    public class TreeNode : Tree
    {
        public List<Tree> Children { get; set; }
    }

    /// <summary>
    /// 叶子节点，代表终止符号
    /// </summary>
    public class TreeLeaf : Tree
    { }
}