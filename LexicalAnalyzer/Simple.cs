using System;
using System.Collections.Generic;

namespace LexicalAnalyzer
{
    /// <summary>
    /// 手写的分析器，推导式：
    /// S → aA
    /// S → cAd
    /// A → a
    /// A → ab
    /// </summary>
    public static class Simple
    {
        /// <summary>
        /// 分析token序列，生成语法树（这里假设一个char是一个 token）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Tree Analyze(string buffer)
        {
            // 起始节点
            var root = new TreeNode {Token = 'S'};

            var success = AnalyzeNode(buffer, 0, root);
            if (!success)
            {
                throw new InvalidOperationException("不是一个合格的语法");
            }

            return root;
        }

        private static bool AnalyzeNode(string buffer, int startIdx, Tree current)
        {
            if (current.Token == 'S')
            {
                var node = current as TreeNode;

                // way 1
                node.Children = new List<Tree>
                {
                    new TreeLeaf
                    {
                        Token = 'a',
                    },
                    new TreeNode
                    {
                        Token = 'A',
                    },
                };

                if (LookupNode(buffer, startIdx, node))
                {
                    return true;
                }

                // way 2
                node.Children = new List<Tree>
                {
                    new TreeLeaf
                    {
                        Token = 'c',
                    },
                    new TreeNode
                    {
                        Token = 'A',
                    },
                    new TreeLeaf
                    {
                        Token = 'd',
                    },
                };

                if (LookupNode(buffer, startIdx, node))
                {
                    return true;
                }

                return false;
            }
            else if (current.Token == 'A')
            {
                var node = current as TreeNode;

                // way 1
                node.Children = new List<Tree>
                {
                    new TreeLeaf{Token = 'a'},
                };

                if (LookupNode(buffer, startIdx, node))
                {
                    return true;
                }

                // way 2
                node.Children = new List<Tree>
                {
                    new TreeLeaf{Token = 'a'},
                    new TreeLeaf{Token = 'b'},
                };

                if (LookupNode(buffer, startIdx, node))
                {
                    return true;
                }

                return false;
            }
            else
            {
                // 终止结点
                return current.Token == buffer[startIdx];
            }
        }

        /// <summary>
        /// 递归查找子树并生成语法树
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="idx"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static bool LookupNode(string buffer, int idx, TreeNode current)
        {
            foreach (var tree in current.Children)
            {
                var finded = AnalyzeNode(buffer, idx++, tree);
                if (!finded)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
