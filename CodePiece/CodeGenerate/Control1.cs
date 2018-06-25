using System.Collections.Generic;
using System.Text;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    public class Control1
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private int _tmpIdx = 0;

        private int _labeldx = 0;

        private LayerTable _layerTable = new LayerTable();

        private string GenTmp()
        {
            return $"t{++_tmpIdx}";
        }

        private string GenLabel()
        {
            return $"L{++_labeldx}";
        }

        private void Emit(string a)
        {
            _stringBuilder.AppendLine(a);
        }

        /// <summary>
        /// 简单一写。这个方法只能在一个对象上执行一次。
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public string Build(Tree tree)
        {
            CodeGen(tree);
            return _stringBuilder.ToString();
        }

        private void CodeGen(Tree tree)
        {
            switch (tree)
            {
                case Exp exp:
                {
                    exp.StrVal = GenTmp();
                    Emit($"{exp.StrVal} = {exp.Boolean.ToString()}");
                    break;
                }
                case IfStmt ifStmt:
                {
                    CodeGen(ifStmt.Test);

                    if (ifStmt.ElsePart != null)
                    {
                        var elseLabel = GenLabel();
                        var endLabel = GenLabel();
                        Emit($"if-false {ifStmt.Test.StrVal} goto {elseLabel}");
                        CodeGen(ifStmt.ThenPart);
                        Emit($"goto {endLabel}");
                        Emit($"label {elseLabel}");
                        CodeGen(ifStmt.ElsePart);
                        Emit($"label {endLabel}");
                    }
                    else
                    {
                        var endLabel = GenLabel();
                        Emit($"if-false {ifStmt.Test.StrVal} goto {endLabel}");
                        CodeGen(ifStmt.ThenPart);
                        Emit($"label {endLabel}");
                    }

                    break;
                }
                case WhileStmt whileStmt:
                {
                    var endLabel = GenLabel();
                    var startLabel = GenLabel();

                    _layerTable.AddLayer(new Dictionary<string, string>
                    {
                        ["while-end-label"] = endLabel,
                    });

                    Emit($"label {startLabel}");
                    CodeGen(whileStmt.Test);
                    Emit($"if-false {whileStmt.Test.StrVal} goto {endLabel}");
                    CodeGen(whileStmt.Body);
                    Emit($"goto {startLabel}");
                    Emit($"label {endLabel}");

                    _layerTable.PopLayer();

                    break;
                }
                case BreakStmt breakStmt:
                {
                    var endLabel = _layerTable.LookUpValue("while-end-label");
                    Emit($"goto {endLabel}");
                    break;
                }
                case OtherStmt otherStmt:
                {
                    Emit(otherStmt.StrVal);
                    break;
                }
            }
        }

        public class Tree
        {
            public string StrVal { get; set; }
        }

        public abstract class Stmt : Tree
        {
        }

        public class IfStmt : Stmt
        {
            public Exp Test { get; set; }

            public Stmt ThenPart { get; set; }

            public Stmt ElsePart { get; set; }
        }

        public class WhileStmt : Stmt
        {
            public Exp Test { get; set; }

            public Stmt Body { get; set; }
        }

        public class BreakStmt : Stmt
        {
        }

        public class OtherStmt : Stmt
        {
        }

        public class Exp : Tree
        {
            public Boolean Boolean { get; set; }
        }

        public enum Boolean
        {
            True,
            False,
        }
    }
}