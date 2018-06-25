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
label L4
t2 = True
if-false t2 goto L3
t3 = False
if-false t3 goto L5
goto L3
goto L6
label L5
Other Codes ...
label L6
goto L4
label L3
goto L2
label L1
label L2
");
        }
    }
}