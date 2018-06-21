using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.CodeGenerate
{
    public class ThreeAddress1Test
    {
        [Fact]
        public void TestGenerate()
        {
            // (1+a)*(b-2)

            var tree = new ThreeAddress1.TreeNode
            {
                Kind = ThreeAddress1.NodeKind.OpKind,
                Op = ThreeAddress1.Optype.Multiply,
                LChild = new ThreeAddress1.TreeNode
                {
                    Kind = ThreeAddress1.NodeKind.OpKind,
                    Op = ThreeAddress1.Optype.Plus,
                    LChild = new ThreeAddress1.TreeNode
                    {
                        Kind = ThreeAddress1.NodeKind.ConstKind,
                        StrVal = "1",
                    },
                    RChild = new ThreeAddress1.TreeNode
                    {
                        Kind = ThreeAddress1.NodeKind.IdKind,
                        StrVal = "a",
                    },
                },
                RChild = new ThreeAddress1.TreeNode
                {
                    Kind = ThreeAddress1.NodeKind.OpKind,
                    Op = ThreeAddress1.Optype.Minus,
                    LChild = new ThreeAddress1.TreeNode
                    {
                        Kind = ThreeAddress1.NodeKind.IdKind,
                        StrVal = "b",
                    },
                    RChild = new ThreeAddress1.TreeNode
                    {
                        Kind = ThreeAddress1.NodeKind.ConstKind,
                        StrVal = "2",
                    }
                }
            };

            var code = new ThreeAddress1()
                .WithTree(tree)
                .Build();

            code.Should().Be(@"t1 = 1 + a
t2 = b - 2
t3 = t1 * t2
");
        }
    }
}