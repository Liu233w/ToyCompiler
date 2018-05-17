using System;
using System.Linq;

namespace Liu233w.Compiler.CompilerFramework.Parser.TreeNodes
{
    /// <summary>
    /// 非终结符的语法分析树
    /// </summary>
    public class NonTerminalTree : ISyntacticAnalysisTree
    {
        /// <summary>
        /// 该非终结符的名称
        /// </summary>
        private readonly string _name;

        public NonTerminalTree(string name, ISyntacticAnalysisTree[] childs, string[] rule)
        {
            _name = name;
            Childs = childs;
            Rule = rule;
        }

        /// <summary>
        /// 该非终结符的子节点
        /// </summary>
        public ISyntacticAnalysisTree[] Childs { get; }

        /// <summary>
        /// 推导出该语法分析树的规则
        /// </summary>
        public string[] Rule { get; }

        public string GetName() => _name;

        public int GetBeginPosition() => Childs[0].GetBeginPosition();

        public int GetEndPosition() => Childs[Childs.Length - 1].GetEndPosition();

        public bool Equals(NonTerminalTree other)
        {
            return _name == other._name && Childs.SequenceEqual(other.Childs) && Rule.SequenceEqual(other.Rule);
        }

        public bool Equals(ISyntacticAnalysisTree other)
        {
            if (other is null) return false;
            return Equals(other as NonTerminalTree);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as NonTerminalTree);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Childs != null ? Childs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Rule != null ? Rule.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}