using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace LexicalAnalyzer.Test
{
    public class Simple_Test
    {
        [Theory]
        [MemberData(nameof(GetCorrectCombinationsForAnalyze))]
        public void Analyze_能够正确分析语法式(string buffer, TreeNode target)
        {
            var res = Simple.Analyze(buffer);

            res.ShouldDeepEqual(target);
        }

        public static IEnumerable<object[]> GetCorrectCombinationsForAnalyze()
        {
            yield return new object[]
            {
                "cabd",
                new TreeNode
                {
                    Token = 'S',
                    Children = new List<Tree>
                    {
                        new TreeLeaf
                        {
                            Token = 'c',
                        },
                        new TreeNode
                        {
                            Token = 'A',
                            Children = new List<Tree>
                            {
                                new TreeLeaf
                                {
                                    Token = 'a',
                                },
                                new TreeLeaf
                                {
                                    Token = 'b',
                                },
                            }
                        },
                        new TreeLeaf
                        {
                            Token = 'd',
                        },
                    }
                }
            };

            yield return new object[]
            {
                "aa",
                new TreeNode
                {
                    Token = 'S',
                    Children = new List<Tree>
                    {
                        new TreeLeaf
                        {
                            Token = 'a',
                        },
                        new TreeNode
                        {
                            Token = 'A',
                            Children = new List<Tree>
                            {
                                new TreeLeaf
                                {
                                    Token = 'a',
                                },
                            }
                        },
                    }
                }
            };

            yield return new object[]
            {
                "aab",
                new TreeNode
                {
                    Token = 'S',
                    Children = new List<Tree>
                    {
                        new TreeLeaf
                        {
                            Token = 'a',
                        },
                        new TreeNode
                        {
                            Token = 'A',
                            Children = new List<Tree>
                            {
                                new TreeLeaf
                                {
                                    Token = 'a',
                                },
                                new TreeLeaf
                                {
                                    Token = 'b',
                                },
                            }
                        },
                    }
                }
            };

            yield return new object[]
            {
                "cad",
                new TreeNode
                {
                    Token = 'S',
                    Children = new List<Tree>
                    {
                        new TreeLeaf
                        {
                            Token = 'c',
                        },
                        new TreeNode
                        {
                            Token = 'A',
                            Children = new List<Tree>
                            {
                                new TreeLeaf
                                {
                                    Token = 'a',
                                },
                            }
                        },
                        new TreeLeaf
                        {
                            Token = 'd',
                        },
                    }
                }
            };

        }

        [Theory]
        [InlineData("zzz")]
        [InlineData("ab")]
        [InlineData("abd")]
        [InlineData("cd")]
        [InlineData("")]
        [InlineData("aa ")]
        public void Analyze_在输入不正确的句子时能够检测到错误(string buffer)
        {
            var res = Simple.Analyze(buffer);
            res.ShouldBeNull();
        }
    }

    public static class TestHelper
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public static void ShouldDeepEqual(this object obj, object that)
        {
            obj.ToJsonString().ShouldBe(that.ToJsonString());
        }
    }
}
