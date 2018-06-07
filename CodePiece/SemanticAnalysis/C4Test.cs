using System.Threading;
using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C4Test
    {
        [Fact]
        public void TestProcessType()
        {
            var s = new C4.S
            {
                Expr = new C4.ExprDiv
                {
                    Left = new C4.ExprDiv
                    {
                        Left = new C4.ExprInt
                        {
                            Num = 5,
                        },
                        Right = new C4.ExprInt
                        {
                            Num = 2,
                        }
                    },
                    Right = new C4.ExprFloat
                    {
                        Num1 = 2,
                        Num2 = 0,
                    }
                }
            };

            C4.ProcessType(s);

            s.Should().BeEquivalentTo(new C4.S
            {
                Type = C4.ExprType.Float,
                Expr = new C4.ExprDiv
                {
                    Type = C4.ExprType.Float,
                    Left = new C4.ExprDiv
                    {
                        Type = C4.ExprType.Float,
                        Left = new C4.ExprInt
                        {
                            Type = C4.ExprType.Float,
                            Num = 5,
                        },
                        Right = new C4.ExprInt
                        {
                            Type = C4.ExprType.Float,
                            Num = 2,
                        }
                    },
                    Right = new C4.ExprFloat
                    {
                        Type = C4.ExprType.Float,
                        Num1 = 2,
                        Num2 = 0,
                    }
                }
            }, opt => opt.RespectingRuntimeTypes());
        }

        [Fact]
        public void TestProcessVal()
        {
            var s = new C4.S
            {
                Expr = new C4.ExprDiv
                {
                    Left = new C4.ExprDiv
                    {
                        Left = new C4.ExprInt
                        {
                            Num = 5,
                        },
                        Right = new C4.ExprInt
                        {
                            Num = 2,
                        }
                    },
                    Right = new C4.ExprFloat
                    {
                        Num1 = 2,
                        Num2 = 0,
                    }
                }
            };

            C4.ProcessType(s);
            C4.ProcessVal(s);

            s.Should().BeEquivalentTo(new C4.S
            {
                Type = C4.ExprType.Float,
                Val = 1.25,
                Expr = new C4.ExprDiv
                {
                    Type = C4.ExprType.Float,
                    Val = 1.25,
                    Left = new C4.ExprDiv
                    {
                        Type = C4.ExprType.Float,
                        Val = 2.5,
                        Left = new C4.ExprInt
                        {
                            Type = C4.ExprType.Float,
                            Num = 5,
                            Val = 5,
                        },
                        Right = new C4.ExprInt
                        {
                            Type = C4.ExprType.Float,
                            Num = 2,
                            Val = 2,
                        }
                    },
                    Right = new C4.ExprFloat
                    {
                        Type = C4.ExprType.Float,
                        Val = 2.0,
                        Num1 = 2,
                        Num2 = 0,
                    }
                }
            }, opt => opt.RespectingRuntimeTypes());
        }
    }
}