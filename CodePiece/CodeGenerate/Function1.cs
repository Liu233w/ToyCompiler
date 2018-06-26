using System;
using System.Collections.Generic;
using System.Text;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    public class Function1
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private int _tmpIdx = 0;

        private int _labeldx = 0;

        /// <summary>
        /// 多层的 Id Table，用于检查某个Id是否在函数参数中定义过。
        /// 如果支持嵌套定义函数的话，这个就有用了。
        /// </summary>
        private readonly IdTable _idTable = new IdTable();

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
                case Program program:
                {
                    foreach (var decl in program.DeclList)
                    {
                        CodeGen(decl);
                    }

                    CodeGen(program.Exp);
                    break;
                }
                case Func func:
                {
                    Emit($"entry {func.Name}");

                    _idTable.AddLayer(new HashSet<string>(func.Argument));

                    CodeGen(func.Body);
                    Emit($"return {func.Body.StrVal}");

                    _idTable.PopLayer();

                    break;
                }
                case OperateExp op:
                {
                    CodeGen(op.Left);
                    CodeGen(op.Right);
                    var tmp = GenTmp();

                    string opStr;
                    switch (op.OpKind)
                    {
                        case OperateKind.Plus:
                            opStr = "+";
                            break;
                        default:
                            throw new Exception("unhandled kind");
                    }

                    Emit($"{tmp} = {op.Left.StrVal} {opStr} {op.Right.StrVal}");
                    op.StrVal = tmp;

                    break;
                }
                case FunCall funCall:
                {
                    foreach (var argument in funCall.Arguments)
                    {
                        CodeGen(argument);
                    }

                    Emit("begin_args");
                    foreach (var argument in funCall.Arguments)
                    {
                        Emit($"arg {argument.StrVal}");
                    }

                    var returned = GenTmp();
                    Emit($"{returned} = call {funCall.FuncName}");

                    funCall.StrVal = returned;
                    break;
                }
                case Id id:
                {
                    if (!_idTable.LookUpId(id.StrVal))
                    {
                        throw new Exception("Id is not defined");
                    }

                    break;
                }
                case Const cst:
                {
                    // do nothing
                    break;
                }
                default:
                    throw new Exception("unhandled tree type");
            }
        }

        public abstract class Tree
        {
            public string StrVal { get; set; }
        }

        public class Program : Tree
        {
            public Func[] DeclList { get; set; }

            public Exp Exp { get; set; }
        }

        public class Func : Tree
        {
            public string Name { get; set; }

            public string[] Argument { get; set; }

            public Exp Body { get; set; }
        }

        public abstract class Exp : Tree
        {
        }

        public class OperateExp : Exp
        {
            public Exp Left { get; set; }

            public Exp Right { get; set; }

            public OperateKind OpKind { get; set; }
        }

        public enum OperateKind
        {
            Plus, // 简单起见就不写其他的了
        }

        public class FunCall : Exp
        {
            public string FuncName { get; set; }

            public Exp[] Arguments { get; set; }
        }

        public class Const : Exp
        {
            // use strval to store
        }

        public class Id : Exp
        {
            // use strval to store
        }
    }
}