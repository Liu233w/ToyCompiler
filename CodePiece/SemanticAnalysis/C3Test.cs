using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.SemanticAnalysis
{
    public class C3Test
    {
        [Fact]
        public void TestCalculate()
        {
            var tree = new C3.Decl
            {
                Type = new C3.Type
                {
                    OType = C3.OType.Int,
                },
                VarList = new C3.VarList
                {
                    Id = new C3.Id
                    {
                        Lexem = "a",
                    },
                    VarList2 = new C3.VarList
                    {
                        Id = new C3.Id
                        {
                            Lexem = "b",
                        },
                        VarList2 = null,
                    }
                }
            };

            C3.CalDType(tree);

            C3.DType.Should().Be("Int");
            C3.DTypeTable.Should().Contain(
                new KeyValuePair<string, string>("a", "Int"),
                new KeyValuePair<string, string>("b", "Int"));
        }
    }
}