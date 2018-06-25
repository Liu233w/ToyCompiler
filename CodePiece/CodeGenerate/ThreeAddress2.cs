using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    /// <summary>
    /// 二维数组三地址码生成（未完成）
    /// </summary>
    public class ThreeAddress2
    {
        private Dictionary<string, string> _symbols;

        private StringBuilder _code;

        private TreeNode _tree;

        private int _tempId;

        public ThreeAddress2 WithTree(TreeNode newTree)
        {
            _tree = newTree;
            return this;
        }

        public string Build()
        {
            _code = new StringBuilder();
            _tempId = 0;
            DoGenerate(_tree);
            return _code.ToString();
        }

        private void DoGenerate(TreeNode node)
        {
            if (node.Kind == NodeKind.OpKind)
            {
                DoGenerate(node.LChild);
                DoGenerate(node.RChild);
                node.StrVal = GetNewTempId();
                string op;

                switch (node.Op)
                {
                    case Optype.Plus:
                        op = "+";
                        break;
                    case Optype.Minus:
                        op = "-";
                        break;
                    case Optype.Multiply:
                        op = "*";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Emit($"{node.StrVal} = {node.LChild.StrVal} {op} {node.RChild.StrVal}");
            }
            else if (node.Kind == NodeKind.AssignKind)
            {
            }
            else if (node.Kind == NodeKind.ArrayKind)
            {
                switch (node)
                {
                    case ArrayNode a:
                    {
                    }
                        break;
                }
            }

            // 不管其他情况
        }

        private void Emit(string codeLine)
        {
            _code.AppendLine(codeLine);
        }

        private string GetNewTempId()
        {
            return $"t{++_tempId}";
        }

        public enum Optype
        {
            Plus,
            Minus,
            Multiply,
        }

        public enum NodeKind
        {
            OpKind,
            ConstKind,
            IdKind,
            ArrayKind,
            AssignKind,
        }

        public class TreeNode
        {
            public NodeKind Kind { get; set; }

            public Optype Op { get; set; }

            public TreeNode LChild { get; set; }

            public TreeNode RChild { get; set; }

            public string StrVal { get; set; }
        }

        public class ArrayNode : TreeNode
        {
            public int Size { get; set; }
        }

        public class TwoDegreeArrayNode : TreeNode
        {
            public int FirstSize { get; set; }

            public int SecondSize { get; set; }
            
            public TreeNode SecondIndex { get; set; }
        }
    }
}