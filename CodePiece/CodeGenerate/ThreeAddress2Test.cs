using Xunit;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    public class ThreeAddress2Test
    {
        [Fact]
        public void TestOneDegreeArray()
        {
            // a[i+1] = b

            var tree = new ThreeAddress2.TreeNode
            {
                Kind = ThreeAddress2.NodeKind.AssignKind,
                LChild = new ThreeAddress2.ArrayNode
                {
                    Kind = ThreeAddress2.NodeKind.ArrayKind,
                    LChild = new ThreeAddress2.TreeNode
                    {
                        Kind = ThreeAddress2.NodeKind.IdKind,
                        StrVal = "a",
                    },
                    RChild = new ThreeAddress2.TreeNode
                    {
                        Kind = ThreeAddress2.NodeKind.OpKind,
                        Op = ThreeAddress2.Optype.Plus,
                        LChild = new ThreeAddress2.TreeNode
                        {
                            Kind = ThreeAddress2.NodeKind.IdKind,
                            StrVal = "i",
                        },
                        RChild = new ThreeAddress2.TreeNode
                        {
                            Kind = ThreeAddress2.NodeKind.ConstKind,
                            StrVal = "1",
                        }
                    }
                },
                RChild = new ThreeAddress2.TreeNode
                {
                    Kind = ThreeAddress2.NodeKind.IdKind,
                    StrVal = "b"
                }
            };

            new ThreeAddress2()
                .WithTree(tree)
                .Build();
        }
    }
}