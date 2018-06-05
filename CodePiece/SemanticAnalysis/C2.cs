using System;
using System.Collections.Generic;
using System.Text;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C2
    {
        public static void CalDType(TreeNode tree)
        {
            switch (tree)
            {
                case Type t:
                {
                    t.DType = t.OType.ToString();
                    break;
                }
                case Decl d:
                {
                    CalDType(d.Type);
                    var type = d.Type.DType;
                    d.DType = type;
                    d.VarList.DType = type;
                    CalDType(d.VarList);
                    break;
                }
                case VarList l:
                {
                    l.Id.DType = l.DType;
                    if (l.VarList2 != null)
                    {
                        l.VarList2.DType = l.DType;
                        CalDType(l.VarList2);
                    }
                    break;
                }
                default:
                {
                    throw new Exception();
                }
            }
        }

        public class TreeNode
        {
            public string DType { get; set; }
        }

        public class Decl : TreeNode
        {
            public Type Type { get; set; }

            public VarList VarList { get; set; }
        }

        public class Type : TreeNode
        {
            public OType OType { get; set; }
        }

        public enum OType
        {
            Int,
            Float,
        }

        public class VarList : TreeNode
        {
            public Id Id { get; set; }

            public VarList VarList2 { get; set; }
        }

        public class Id : TreeNode
        {
            public string Lexem { get; set; }
        }
    }
}
