using System;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public static class C1
    {
        public static int CalVal(TreeNode node)
        {
            if (node == null)
            {
                return 0;
            }

            if (node is ConstNode n)
            {
                return n.Val;
            }
            else if (node is OpNode t)
            {
                switch (t.Type)
                {
                    case OpType.Plus:
                    {
                        return CalVal(t.Left) + CalVal(t.Right);
                    }
                    case OpType.Minus:
                    {
                        return CalVal(t.Left) - CalVal(t.Right);
                    }

                    default:
                    {
                        throw new Exception();
                    }
                }
            }
            else
            {
                throw new Exception();
            }
        }

        public class TreeNode
        {
        }

        public class ConstNode : TreeNode
        {
            public int Val { get; set; }
        }

        public enum OpType
        {
            Plus,
            Minus,
        }

        public class OpNode : TreeNode
        {
            public OpType Type { get; set; }

            public TreeNode Left { get; set; }

            public TreeNode Right { get; set; }
        }
    }
}