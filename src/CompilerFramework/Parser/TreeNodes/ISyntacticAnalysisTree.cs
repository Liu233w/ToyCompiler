using System;

namespace Liu233w.Compiler.CompilerFramework.Parser.TreeNodes
{
    /// <summary>
    /// 语法分析树
    /// </summary>
    public interface ISyntacticAnalysisTree : IEquatable<ISyntacticAnalysisTree>
    {
        /// <summary>
        /// 树的名称，有可能是非终结符的名字或 TokenType
        /// </summary>
        string GetName();

        /// <summary>
        /// 获取语法分析树起点在源代码中的位置
        /// </summary>
        int GetBeginPosition();

        /// <summary>
        /// 获取语法分析树终点的下一个字符在源代码中的位置
        /// </summary>
        int GetEndPosition();
    }
}