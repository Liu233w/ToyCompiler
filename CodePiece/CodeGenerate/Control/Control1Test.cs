using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.CodeGenerate.Control
{
    public class Control1Test
    {
        [Fact]
        public void TestControl()
        {
            var tree = new Control1.IfStmt
            {
                Test = new Control1.Exp
                {
                    Boolean = Control1.Boolean.True,
                },
                ThenPart = new Control1.WhileStmt
                {
                    Test = new Control1.Exp
                    {
                        Boolean = Control1.Boolean.True
                    },
                    Body = new Control1.IfStmt
                    {
                        Test = new Control1.Exp
                        {
                            Boolean = Control1.Boolean.False,
                        },
                        ThenPart = new Control1.BreakStmt(),
                        ElsePart = new Control1.OtherStmt
                        {
                            StrVal = "Other Codes ..."
                        }
                    }
                }
            };

            new Control1()
                .Build(tree)
                .Should()
                .Be(@"t1 = True
if-false t1 goto L1
label L3
t2 = True
if-false t2 goto L2
t3 = False
if-false t3 goto L4
goto L2
goto L5
label L4
Other Codes ...
label L5
goto L3
label L2
label L1
");
        }
    }
}