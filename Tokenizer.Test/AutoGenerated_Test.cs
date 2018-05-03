using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Tokenizer.Test
{
    public class AutomataTokenizer_Test
    {
        private readonly AutomataState _automataSingle;

        private readonly AutomataState _nameWithComment;

        public AutomataTokenizer_Test()
        {
            var endState = new AutomataState
            {
                Asserter = char.IsLetterOrDigit,
                IdentifierType = "name",
            };
            endState.Next.Add(endState);

            _automataSingle = new AutomataState
            {
                Next = new List<AutomataState>
                {
                    new AutomataState
                    {
                        Asserter = char.IsLetter,
                        Next = { endState, }
                    }
                }
            };

            var commentBody = new AutomataState
            {
                Asserter = c => true,
                Next =
                {
                    new AutomataState
                    {
                        Asserter = c => c == '*',
                        Next =
                        {
                            new AutomataState
                            {
                                Asserter = c => c == '/',
                                IdentifierType = "comment",
                            }
                        }
                    },
                    // commentBody,
                }
            };
            commentBody.Next.Add(commentBody);

            _nameWithComment = new AutomataState
            {
                Asserter = null,
                IdentifierType = null,
                Next =
                {
                    new AutomataState
                    {
                        Asserter = char.IsLetter,
                        IdentifierType = null,
                        Next =
                        {
                            endState,
                        }
                    },
                    new AutomataState
                    {
                        Asserter = c => c == '/',
                        IdentifierType = null,
                        Next =
                        {
                            new AutomataState
                            {
                                Asserter = c => c == '*',
                                IdentifierType = null,
                                Next = { commentBody }
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void 应该能够识别名字()
        {
            var res = AutomataTokenizer.GetByAutomata(_automataSingle, "name1", 0);
            res.Recognized.ShouldBe("name1");
            res.LastPosition.ShouldBe(5);
            res.IdentifierType.ShouldBe("name");
        }

        [Fact]
        public void 不能识别的时候应该返回空()
        {
            var res = AutomataTokenizer.GetByAutomata(_automataSingle, "123", 0);
            res.Recognized.ShouldBeNull();
            res.LastPosition.ShouldBe(0);
            res.IdentifierType.ShouldBeNull();
        }

        [Fact]
        public void 可以识别目前识别的子串()
        {
            var res = AutomataTokenizer.GetByAutomata(_automataSingle, "name1.1", 0);
            res.Recognized.ShouldBe("name1");
            res.LastPosition.ShouldBe(5);
            res.IdentifierType.ShouldBe("name");
        }

        [Fact]
        public void 能够识别单个字符作为的名字()
        {
            var res = AutomataTokenizer.GetByAutomata(_automataSingle, "a", 0);
            res.Recognized.ShouldBe("a");
            res.IdentifierType.ShouldBe("name");
            res.LastPosition.ShouldBe(1);
        }

        [Fact]
        public void 在包含多个终止状态时能够正确识别_name在前()
        {
            string buffer = "aaa/*asddg*/";

            var res = AutomataTokenizer.GetByAutomata(_nameWithComment, buffer, 0);
            res.Recognized.ShouldBe("aaa");
            res.IdentifierType.ShouldBe("name");
            res.LastPosition.ShouldBe(3);

            res = AutomataTokenizer.GetByAutomata(_nameWithComment, buffer, res.LastPosition);
            res.Recognized.ShouldBe("/*asddg*/");
            res.IdentifierType.ShouldBe("comment");
            res.LastPosition.ShouldBe(12);
        }

        [Fact]
        public void 在包含多个终止状态时能够正确识别_comment在前()
        {
            string buffer = "/*asddg*/aaa";

            var res = AutomataTokenizer.GetByAutomata(_nameWithComment, buffer, 0);
            res.Recognized.ShouldBe("/*asddg*/");
            res.IdentifierType.ShouldBe("comment");
            res.LastPosition.ShouldBe(9);

            res = AutomataTokenizer.GetByAutomata(_nameWithComment, buffer, res.LastPosition);
            res.Recognized.ShouldBe("aaa");
            res.IdentifierType.ShouldBe("name");
            res.LastPosition.ShouldBe(12);
        }
    }
}