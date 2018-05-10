using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer
{
    public class SimpleParam : ICloneable
    {
        public int StartIdx { get; set; }

        public Tree Current { get; set; }

        /// <summary>
        /// 在复制时不对 Tree 进行深拷贝，而是直接复制引用，便于在恢复续延时对相同的对象保持引用
        /// </summary>
        public object Clone()
        {
            return new SimpleParam
            {
                StartIdx = StartIdx,
                Current = Current
            };
        }
    }

    public class SimpleVariable : ICloneable
    {

        public int RuleIdx { get; set; }

        public TreeNode Tree { get; set; }

        public int ChildIdx { get; set; }

        /// <summary>
        /// 在复制时不对 Tree 进行深拷贝，而是直接复制引用，便于在恢复续延时对相同的对象保持引用
        /// </summary>
        public object Clone()
        {
            return new SimpleVariable
            {
                ChildIdx = ChildIdx,
                Tree = Tree,
                RuleIdx = RuleIdx,
            };
        }
    }

    /// <summary>
    /// 手写的分析器，推导式：
    /// S → aA
    /// S → cAd
    /// A → a
    /// A → ab
    ///
    /// 在实现上，本类在外部手动模拟了函数的调用栈，以实现类似 call/cc 的效果。
    /// 为了可读性，不使用 cps 变换来写这个算法。
    ///
    /// 如果不能解析语法树，返回 null
    /// </summary>
    public class Simple : StackMachine<SimpleParam, SimpleVariable, bool>
    {
        /// <summary>
        /// 分析token序列，生成语法树（这里假设一个char是一个 token）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static TreeNode Analyze(string buffer)
        {
            var function = new Simple(buffer);

            // 起始节点
            var root = new TreeNode { Token = 'S' };

            var success = function.Run(new SimpleParam { Current = root, StartIdx = 0 });
            while (!success)
            {
                if (function._backtracings.Count == 0)
                {
                    return null;
                }

                var continuation = function._backtracings.Pop();
                success = function.ResumeAtContinuation(continuation, true);
            }

            Debug.Assert(root != null, "最后返回处的语法树应该为 TreeNode");
            return root;
        }


        class Rule
        {
            public Rule(char from, string to)
            {
                From = from;
                To = to;
            }

            public char From { get; set; }

            public string To { get; set; }
        }

        private readonly List<Rule> _rules;
        private readonly string _buffer;
        private readonly Stack<Continuation> _backtracings;

        private Simple(string buffer)
        {
            _rules = new List<Rule>
            {
                new Rule('S', "aA"),
                new Rule('S', "cAd"),
                new Rule('A', "a"),
                new Rule('A', "ab"),
            };

            _buffer = buffer;
            _backtracings = new Stack<Continuation>();
        }

        protected override bool StepMove(
            SimpleParam p,
            SimpleVariable v,
            int pc,
            bool lastReturned,
            object callCCReturned)
        {
            // 根据程序计数器决定执行哪一段代码
            switch (pc)
            {
                case 0:
                {
                    // 使用花括号来避免在多个 case 之间共享状态
                    if (p.Current is TreeLeaf leaf)
                    {
                        // 根据语法树，当前应当是终结符号
                        return Return(_buffer[p.StartIdx] == leaf.Token);
                    }

                    // 确保语法树的当前节点是 非终结符
                    // 根据定义，起始符号S必须是非终结符，因此可以直接处理
                    v.Tree = p.Current as TreeNode;
                    if (v.Tree == null)
                    {
                        return Return(false);
                    }

                    // 开始循环(入口点 1)
                    v.RuleIdx = 0;

                    return Continue();
                }
                case 1:
                {
                    // 在规则中找到一个匹配的产生式，如果产生式不存在，返回 false
                    while (v.RuleIdx < _rules.Count)
                    {
                        if (v.Tree.Token == _rules[v.RuleIdx].From)
                        {
                            return Continue();
                        }
                        else
                        {
                            ++v.RuleIdx;
                        }
                    }

                    return Return(false);
                }
                case 2:
                {
                    // 保存当前调用栈状态，便于以后回溯
                    return CallCC(cont =>
                    {
                        _backtracings.Push(cont);
                        return false;
                    });
                }
                case 3:
                {
                    var shouldSkipTheRule = (bool) callCCReturned;
                    if (shouldSkipTheRule)
                    {
                        ++v.RuleIdx;
                        return GoTo(1);
                    }

                    // 根据产生式规则来设定语法树的子节点
                    v.Tree.Children = new List<Tree>();
                    foreach (var c in _rules[v.RuleIdx].To)
                    {
                        if (char.IsUpper(c))
                        {
                            // 假设大写的都是非终结符号，小写的都是终结符号
                            v.Tree.Children.Add(new TreeNode { Token = c });
                        }
                        else
                        {
                            v.Tree.Children.Add(new TreeLeaf { Token = c });
                        }
                    }

                    // 开始循环(入口点 4)
                    v.ChildIdx = 0;

                    return Continue();
                }
                case 4:
                {
                    Debug.Assert(v.Tree.Children.Count > 0, "当前情况下每个非终止符应当能够产生多个符号");
                    if (v.ChildIdx >= v.Tree.Children.Count)
                    {
                        // 检查完了所有子节点
                        return Return(true);
                    }

                    return RecursiveCall(new SimpleParam
                    {
                        Current = v.Tree.Children[v.ChildIdx],
                        StartIdx = p.StartIdx + v.ChildIdx,
                    });
                }
                case 5:
                {
                    var success = lastReturned;
                    if (!success)
                    {
                        return Return(false);
                    }

                    ++v.ChildIdx;

                    // 这里没法用循环，只能用 goto 了。
                    return GoTo(4);
                }
                default:
                {
                    throw new Exception("无效的程序计数器状态");
                }
            }
        }
    }
}
