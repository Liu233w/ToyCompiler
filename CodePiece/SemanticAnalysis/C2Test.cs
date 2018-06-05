using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C2Test
    {
        [Fact]
        public void TestCalculate()
        {
            var tree = new C2.Decl
            {
                Type = new C2.Type
                {
                    OType = C2.OType.Int,
                },
                VarList = new C2.VarList
                {
                    Id = new C2.Id
                    {
                        Lexem = "a",
                    },
                    VarList2 = new C2.VarList
                    {
                        Id = new C2.Id
                        {
                            Lexem = "b",
                        },
                        VarList2 = null,
                    }
                }
            };

            C2.CalDType(tree);

            tree.Should().BeEquivalentTo(new C2.Decl
            {
                Type = new C2.Type
                {
                    OType = C2.OType.Int,
                    DType = "Int",
                },
                VarList = new C2.VarList
                {
                    Id = new C2.Id
                    {
                        Lexem = "a",
                        DType = "Int",
                    },
                    VarList2 = new C2.VarList
                    {
                        Id = new C2.Id
                        {
                            Lexem = "b",
                            DType = "Int",
                        },
                        VarList2 = null,
                        DType = "Int",
                    },
                    DType = "Int",
                },
                DType = "Int",
            });
        }
    }
}