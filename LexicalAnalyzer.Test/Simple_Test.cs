using System.Collections.Generic;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace LexicalAnalyzer.Test
{
    public class Simple_Test
    {
        private readonly Simple _analyzer;

        public Simple_Test()
        {
            _analyzer = new Simple();
        }

        [Fact]
        public void Analyze_能够正确分析语法式()
        {
            const string buffer = "cabd";

            var res = _analyzer.Analyze(buffer);

            res.ShouldDeepEqual(new TreeNode
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
            });
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
