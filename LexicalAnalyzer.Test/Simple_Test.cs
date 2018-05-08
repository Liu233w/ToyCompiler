using System;
using System.Collections.Generic;
using DeepEqual.Syntax;
using Shouldly;
using Xunit;

namespace LexicalAnalyzer.Test
{
    public class Simple_Test
    {
        [Fact]
        public void Analyze_能够正确分析语法式()
        {
            var buffer = "cabd";

            var res = Simple.Analyze(buffer);

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
}
