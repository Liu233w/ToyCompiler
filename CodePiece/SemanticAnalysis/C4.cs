using System;
using System.Diagnostics;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C4
    {
        public static void ProcessType(Node node)
        {
            var type = GetType(node);
            SetType(node, type);
        }

        private static ExprType GetType(Node n)
        {
            switch (n)
            {
                case S s:
                {
                    return GetType(s.Expr);
                }
                case ExprDiv div:
                {
                    var left = GetType(div.Left);
                    var right = GetType(div.Right);
                    if (left == ExprType.Float || right == ExprType.Float)
                    {
                        return ExprType.Float;
                    }
                    else
                    {
                        return ExprType.Int;
                    }
                }
                case ExprFloat f:
                {
                    return ExprType.Float;
                }
                case ExprInt i:
                {
                    return ExprType.Int;
                }
                default:
                {
                    throw new Exception();
                }
            }
        }

        private static void SetType(Node node, ExprType type)
        {
            node.Type = type;
            switch (node)
            {
                case S s:
                {
                    SetType(s.Expr, type);
                    break;
                }
                case ExprDiv div:
                {
                    SetType(div.Left, type);
                    SetType(div.Right, type);
                    break;
                }
            }
        }

        public static void ProcessVal(Node node)
        {
            switch (node)
            {
                case S s:
                {
                    ProcessVal(s.Expr);
                    s.Val = s.Expr.Val;
                    break;
                }
                case ExprDiv div:
                {
                    ProcessVal(div.Left);
                    ProcessVal(div.Right);

                    if (div.Type == ExprType.Float)
                    {
                        div.Val = FloatDiv(div.Left.Val, div.Right.Val);
                    }
                    else
                    {
                        div.Val = IntDiv(div.Left.Val, div.Right.Val);
                    }

                    break;
                }
                case ExprInt i:
                {
                    if (i.Type == ExprType.Float)
                    {
                        i.Val = Float(i.Num);
                    }
                    else
                    {
                        i.Val = i.Num;
                    }

                    break;
                }
                case ExprFloat f:
                {
                    f.Val = f.Num1 + double.Parse("0." + f.Num2.ToString());
                    break;
                }
            }
        }

        public static object FloatDiv(object left, object right)
        {
            return (double) left / (double) right;
        }

        public static object IntDiv(object left, object right)
        {
            return (int) left / (int) right;
        }

        public static object Float(object a)
        {
            double b = (int) a;
            return b;
        }

        public class Node
        {
            public object Val { get; set; }

            public ExprType Type { get; set; }
        }

        public class S : Node
        {
            public Expr Expr { get; set; }
        }

        public abstract class Expr : Node
        {
        }

        public class ExprDiv : Expr
        {
            public Expr Left { get; set; }

            public Expr Right { get; set; }
        }

        public class ExprInt : Expr
        {
            public int Num { get; set; }
        }

        public class ExprFloat : Expr
        {
            public int Num1 { get; set; }

            public int Num2 { get; set; }
        }

        public enum ExprType
        {
            Int,
            Float,
        }
    }
}