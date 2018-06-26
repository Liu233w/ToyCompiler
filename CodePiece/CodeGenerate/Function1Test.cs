using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    public class Function1Test
    {
        [Fact]
        public void TestBuild()
        {
            var tree = new Function1.Program
            {
                DeclList = new Function1.Func[]
                {
                    new Function1.Func
                    {
                        Name = "f",
                        Argument = new string[] {"x"},
                        Body = new Function1.OperateExp
                        {
                            OpKind = Function1.OperateKind.Plus,
                            Left = new Function1.Const
                            {
                                StrVal = "2"
                            },
                            Right = new Function1.Id
                            {
                                StrVal = "x",
                            }
                        }
                    },
                    new Function1.Func
                    {
                        Name = "g",
                        Argument = new string[] {"x", "y"},
                        Body = new Function1.OperateExp
                        {
                            OpKind = Function1.OperateKind.Plus,
                            Left = new Function1.FunCall
                            {
                                FuncName = "f",
                                Arguments = new Function1.Exp[]
                                {
                                    new Function1.Id
                                    {
                                        StrVal = "x"
                                    }
                                },
                            },
                            Right = new Function1.Id
                            {
                                StrVal = "y"
                            }
                        },
                    }
                },
                Exp = new Function1.FunCall
                {
                    FuncName = "g",
                    Arguments = new Function1.Exp[]
                    {
                        new Function1.Const
                        {
                            StrVal = "3"
                        },
                        new Function1.Const
                        {
                            StrVal = "4"
                        }
                    }
                }
            };

            new Function1()
                .Build(tree)
                .Should()
                .Be(@"entry f
t1 = 2 + x
return t1
entry g
begin_args
arg x
t2 = call f
t3 = t2 + y
return t3
begin_args
arg 3
arg 4
t4 = call g
");
        }
    }
}