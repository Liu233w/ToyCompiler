using System;
using System.Collections;
using System.Collections.Generic;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C3
    {
        public static Dictionary<string, string> DTypeTable = new Dictionary<string, string>();

        public static string DType;
        
        public static void CalDType(TreeNode tree)
        {
            switch (tree)
            {
                case Type t:
                {
                    DType = t.OType.ToString();
                    break;
                }
                case Decl d:
                {
                    CalDType(d.Type);
                    CalDType(d.VarList);
                    break;
                }
                case VarList l:
                {
                    DTypeTable[l.Id.Lexem] = DType;
                    if (l.VarList2 != null)
                    {
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