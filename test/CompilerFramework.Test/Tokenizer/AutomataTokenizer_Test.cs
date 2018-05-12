using System.Collections.Generic;
using System.Linq;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Shouldly;
using Xunit;

namespace Liu233w.Compiler.CompilerFramework.Test.Tokenizer
{
    // ReSharper disable once InconsistentNaming
    public class AutomataTokenizer_Test
    {
        private readonly AutomataTokenizerState _nameState;

        private readonly AutomataTokenizerState _nameWithCommentState;

        private readonly AutomataTokenizerState _certainState;

        public AutomataTokenizer_Test()
        {
            var nameEndState = AutomataTokenizerState.ForEnd(char.IsLetterOrDigit, "name");
            nameEndState.NextStates.Add(nameEndState);

            _nameState = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForEnd(char.IsLetter, "name", new List<AutomataTokenizerState>{nameEndState}),
            });

            var commentEnd = AutomataTokenizerState.ForMiddle('*'.MatchCurrentPosition(),
                new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForEnd('/'.MatchCurrentPosition(), "comment"),
                });

            var commentBody = AutomataTokenizerState.ForMiddle(c => true, new List<AutomataTokenizerState>
            {
                commentEnd,
                // commentBody
            });
            commentBody.NextStates.Add(commentBody);

            _nameWithCommentState = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForEnd(char.IsLetter, "name", new List<AutomataTokenizerState> {nameEndState}),
                AutomataTokenizerState.ForMiddle('/'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForMiddle('*'.MatchCurrentPosition(),
                        new List<AutomataTokenizerState>
                        {
                            // 状态机是按顺序遍历的，假如将 body 放在前面，就不会回溯了
                            commentEnd,
                            commentBody,
                        }),
                })
            });

            _certainState = AutomataTokenizerState.ForBegin(new List<AutomataTokenizerState>
            {
                AutomataTokenizerState.ForMiddle('1'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                {
                    AutomataTokenizerState.ForMiddle('2'.MatchCurrentPosition(), new List<AutomataTokenizerState>
                    {
                        AutomataTokenizerState.ForEnd('3'.MatchCurrentPosition(), "certain")
                    })
                })
            });
        }

        #region GetByAutomata

        [Fact]
        public void GetByAutomata_应该能够识别名字()
        {
            var res = AutomataTokenizer.GetByAutomata(_nameState, "name1", 0, out var end);

            res.ShouldMatchRight(new Token("name1", "name", 0, 5));
            end.ShouldBe(5);
        }

        [Fact]
        public void GetByAutomata_不能识别的时候应返回异常()
        {
            AutomataTokenizer.GetByAutomata(_nameState, "123", 0, out _)
                .ShouldBeLeft(exception =>
                {
                    exception.Buffer.ShouldBe("123");
                    exception.TokenBegin.ShouldBe(0);
                    exception.CurrentIdx.ShouldBe(0);
                    exception.CurrentState.ShouldBe(_nameState);
                });
        }

        [Fact]
        public void GetByAutomata_可以识别目前识别的子串()
        {
            AutomataTokenizer.GetByAutomata(_nameState, "name1.1", 0, out var end)
                .ShouldMatchRight(new Token("name1", "name", 0, 5));

            end.ShouldBe(5);
        }

        [Fact]
        public void GetByAutomata_能够识别单个字符作为的名字()
        {
            AutomataTokenizer.GetByAutomata(_nameState, "a", 0, out var end)
                .ShouldMatchRight(new Token("a", "name", 0, 1));

            end.ShouldBe(1);
        }

        [Theory]
        [InlineData("a", 0, 0)]
        [InlineData("1a", 1, '1')]
        [InlineData("12a", 2, '2')]
        public void GetByAutomata_在无法识别时能够返回含有终点位置的异常(string buffer, int expectEndAt, char expectStateAt)
        {
            // 使用有限状态机进行遍历不应该有回溯操作，所以这里应该直接返回已经识别的Token
            AutomataTokenizer.GetByAutomata(_certainState, buffer, 0, out var nextBeginIdx)
                .ShouldBeLeft(exception =>
                {
                    exception.CurrentIdx.ShouldBe(expectEndAt);
                    exception.TokenBegin.ShouldBe(0);
                    if (expectStateAt == 0)
                    {
                        exception.CurrentState.Asserter.ShouldBeNull();
                    }
                    else
                    {
                        exception.CurrentState.Asserter(expectStateAt).ShouldBeTrue();
                    }
                });

            nextBeginIdx.ShouldBe(expectEndAt);
        }

        [Fact]
        public void GetByAutomata_在包含多个终止状态时能够正确识别_name在前()
        {
            const string buffer = "aaa/*asddg*/";

            var res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, 0, out var end);
            res.ShouldMatchRight(new Token("aaa", "name", 0, 3));
            end.ShouldBe(3);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchRight(new Token("/*asddg*/", "comment", 3, 12));
            end.ShouldBe(12);
        }

        [Fact]
        public void GetByAutomata_在包含多个终止状态时能够正确识别_comment在前()
        {
            const string buffer = "/*asddg*/aaa";

            var res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, 0, out var end);
            res.ShouldMatchRight(new Token("/*asddg*/", "comment", 0, 9));
            end.ShouldBe(9);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchRight(new Token("aaa", "name", 9, 12));
            end.ShouldBe(12);
        }

        [Fact]
        public void GetByAutomata_在包含多个终止状态时能够正确识别_夹心()
        {
            const string buffer = "aaa/*asddg*/bbb";

            var res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, 0, out var end);
            res.ShouldMatchRight(new Token("aaa", "name", 0, 3));
            end.ShouldBe(3);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchRight(new Token("/*asddg*/", "comment", 3, 12));
            end.ShouldBe(12);

            res = AutomataTokenizer.GetByAutomata(_nameWithCommentState, buffer, end, out end);
            res.ShouldMatchRight(new Token("bbb", "name", 12, 15));
            end.ShouldBe(15);
        }

        #endregion

        #region GetAllTokenByAutomata

        [Fact]
        public void GetAllTokenByAutomata_能够获取Token流()
        {
            var res = AutomataTokenizer.GetAllTokenByAutomata(_nameWithCommentState, "/*aaaaa*/abc");

            res.Rights().ToArray().ShouldMatchObject(new[]
            {
                new Token("/*aaaaa*/", "comment", 0, 9),
                new Token("abc", "name", 9, 12),
            });
        }

        [Fact]
        public void GetAllTokenByAutomata_能够获取到大段文字的Token流()
        {
            var whiteSpace = AutomataTokenizerState.ForEnd(' '.MatchCurrentPosition(), "space");
            whiteSpace.NextStates.Add(whiteSpace);

            _nameWithCommentState.NextStates.Add(whiteSpace);

            const string buffer = "abc def ggg /*fdsafdsf  fff  */ llll a/**/a ";
            var res = AutomataTokenizer.GetAllTokenByAutomata(_nameWithCommentState, buffer).Rights();

            var enumerable = res as Token[] ?? res.ToArray();
            enumerable.Select(item => item.TokenType).ToArray().ShouldMatchObject(new[]
            {
                "name", "space", "name", "space", "name", "space",
                "comment", "space", "name", "space", "name",
                "comment", "name", "space",
            });
            enumerable.Select(item => item.Content).ToArray().ShouldMatchObject(new[]
            {
                "abc", " ", "def", " ", "ggg", " ", "/*fdsafdsf  fff  */",
                " ", "llll", " ", "a", "/**/", "a", " ",
            });
        }

        #endregion
    }
}