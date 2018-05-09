using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json;

namespace LexicalAnalyzer
{
    /// <summary>
    /// 手写的分析器，推导式：
    /// S → aA
    /// S → cAd
    /// A → a
    /// A → ab
    ///
    /// 在实现上，本类在外部手动模拟了函数的调用栈，以实现类似 call/cc 的效果。
    /// 为了可读性，不使用 cps 变换来写这个算法。
    /// </summary>
    public class Simple
    {

        #region 调用栈相关

        /// <summary>
        /// 一个栈帧。用于人工模拟的调用栈
        /// </summary>
        class CallingStackFrame
        {
            public CallingStackFrame(int startIdx, Tree current)
            {
                StartIdx = startIdx;
                Current = current;

                ChildIdx = 0;
                RuleIdx = 0;
                Tree = null;
                Pc = 0;
            }

            #region 函数参数

            public int StartIdx { get; set; }

            public Tree Current { get; set; }

            #endregion

            #region 函数内部变量

            public int RuleIdx { get; set; }

            public TreeNode Tree { get; set; }

            public int ChildIdx { get; set; }

            #endregion

            #region 返回值

            /// <summary>
            /// 函数返回值，如果为 null 代表函数还没有返回
            /// </summary>
            public bool? Result { get; set; }

            #endregion

            /// <summary>
            /// 当前函数的程序计数器，表示运行到了哪一部分。在恢复函数状态时使用。
            /// </summary>
            public int Pc { get; set; }
        }

        #region 在DoAnalyzeByStep函数内部使用的状态函数

        /// <summary>
        /// 退出当前函数
        /// </summary>
        /// <param name="result">当前函数的返回值</param>
        private bool Return(bool result)
        {
            var frame = _callingStack.Peek();
            frame.Result = result;
            frame.Pc = -1;

            return true;
        }

        /// <summary>
        /// 调用下个函数，在调用之前先设置返回点
        /// </summary>
        /// <param name="pc">返回点。在下个函数返回之后进入哪个位置</param>
        /// <param name="startIdx"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool CallNextFunctionAndSetReturnPoint(int pc, int startIdx, Tree current)
        {
            _callingStack.Peek().Pc = pc;
            _callingStack.Push(new CallingStackFrame(startIdx, current));
            return false;
        }

        /// <summary>
        /// 跳到下一个状态
        /// </summary>
        /// <returns></returns>
        private bool GoToNextPc()
        {
            ++_callingStack.Peek().Pc;
            return false;
        }

        /// <summary>
        /// 跳到某个状态
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private bool GoTo(int pc)
        {
            _callingStack.Peek().Pc = pc;
            return false;
        }

        #endregion

        /// <summary>
        /// 保存当前上下文
        /// </summary>
        private void SaveCurrentContinuation()
        {
            var snapshot = JsonConvert.SerializeObject(
                _callingStack,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });
            _callingStackSnapshots.Push(snapshot);
        }

        /// <summary>
        /// 从最近的续延处重启运算，同时改变一些状态。假如没有最近的续延，返回 false
        /// </summary>
        /// <returns></returns>
        private bool TryContinueWithRecentContinuation()
        {
            if (_callingStackSnapshots.Count <= 0)
            {
                return false;
            }

            var snapshot = _callingStackSnapshots.Pop();
            _callingStack = JsonConvert.DeserializeObject<Stack<CallingStackFrame>>(
                snapshot,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });

            ++_callingStack.Peek().RuleIdx;

            return true;
        }

        #endregion

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

        private List<Rule> _rules;
        private Stack<CallingStackFrame> _callingStack;
        private Stack<string> _callingStackSnapshots;
        private string _buffer;

        public Simple()
        {
            _rules = new List<Rule>
            {
                new Rule('S', "aA"),
                new Rule('S', "cAd"),
                new Rule('A', "a"),
                new Rule('A', "ab"),
            };
        }

        private void ResetStack()
        {
            _callingStack = new Stack<CallingStackFrame>();
            _callingStackSnapshots = new Stack<string>();
        }

        /// <summary>
        /// 分析token序列，生成语法树（这里假设一个char是一个 token）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public TreeNode Analyze(string buffer)
        {
            _buffer = buffer;
            ResetStack();

            // 起始节点
            var root = new TreeNode { Token = 'S' };
            _callingStack.Push(new CallingStackFrame(0, root));

            do
            {
                root = _callingStack.Peek().Current as TreeNode;
                var success = ExecuteAndGetResult();

                if (success)
                {
                    Debug.Assert(root != null, "最后返回处的语法树应该为 TreeNode");
                    return root;
                }
            } while (TryContinueWithRecentContinuation());

            throw new InvalidOperationException("不是一个合格的语法");
        }

        /// <summary>
        /// 在模拟的调用栈上运行函数并得到结果
        /// </summary>
        /// <returns></returns>
        private bool ExecuteAndGetResult()
        {
            // 上次运算的结果
            bool? stackReturnedResult = null;

            while (_callingStack.Count > 0)
            {
                var done = DoAnalyzeByStep(stackReturnedResult);
                stackReturnedResult = null;

                if (!done) continue;

                // 当前函数已返回，该退栈了
                stackReturnedResult = _callingStack.Pop().Result;
                Debug.Assert(stackReturnedResult.HasValue, "函数应当有返回值");
            }

            Debug.Assert(stackReturnedResult != null, "至少运行了一次");
            return stackReturnedResult.Value;
        }

        /// <summary>
        /// 从模拟的调用栈运行分析函数。参数从调用栈中获取，返回值会被压入到调用栈中。
        /// </summary>
        /// <param name="stackReturnedResult">
        /// 递归调用的函数退栈时的返回值。
        /// 在函数退栈时，上个函数的返回值应当作为此参数传进来。
        /// </param>
        /// <returns>
        /// 当前函数的控制流是否结束。
        /// 如果函数递归调用了下一个函数，会将该函数的栈帧压入到调用栈中，并返回 true；
        /// 如果函数返回，会在栈帧中设置本函数的返回值，并返回 false。
        /// </returns>
        private bool DoAnalyzeByStep(bool? stackReturnedResult = null)
        {
            // 当前函数栈帧
            var frame = _callingStack.Peek();

            // 根据程序计数器决定执行哪一段代码
            switch (frame.Pc)
            {
                case 0:
                {
                    // 使用花括号来避免在多个 case 之间共享状态
                    if (frame.Current is TreeLeaf leaf)
                    {
                        // 根据语法树，当前应当是终结符号
                        return Return(_buffer[frame.StartIdx] == leaf.Token);
                    }

                    // 确保语法树的当前节点是 非终结符
                    // 根据定义，起始符号S必须是非终结符，因此可以直接处理
                    frame.Tree = frame.Current as TreeNode;
                    if (frame.Tree == null)
                    {
                        return Return(false);
                    }

                    return GoToNextPc();
                }
                case 1:
                {
                    // 在规则中找到一个匹配的产生式，如果产生式不存在，返回 false
                    while (frame.RuleIdx < _rules.Count)
                    {
                        if (frame.Tree.Token == _rules[frame.RuleIdx].From)
                        {
                            return GoToNextPc();
                        }
                        else
                        {
                            ++frame.RuleIdx;
                        }
                    }

                    return Return(false);
                }
                case 2:
                {
                    // 保存当前调用栈状态，便于以后回溯
                    SaveCurrentContinuation();

                    // 根据产生式规则来设定语法树的子节点
                    frame.Tree.Children = new List<Tree>();
                    foreach (var c in _rules[frame.RuleIdx].To)
                    {
                        if (char.IsUpper(c))
                        {
                            // 假设大写的都是非终结符号，小写的都是终结符号
                            frame.Tree.Children.Add(new TreeNode { Token = c });
                        }
                        else
                        {
                            frame.Tree.Children.Add(new TreeLeaf { Token = c });
                        }
                    }

                    return GoToNextPc();
                }
                case 3:
                {
                    // default ChildIndex = 0
                    if (frame.ChildIdx >= frame.Tree.Children.Count)
                    {
                        // 检查完了所有子节点
                        return Return(true);
                    }

                    return CallNextFunctionAndSetReturnPoint(4, frame.StartIdx + frame.ChildIdx,
                        frame.Tree.Children[frame.ChildIdx]);
                }
                case 4:
                {
                    Debug.Assert(stackReturnedResult.HasValue, "上一个函数调用应当返回值");
                    var success = stackReturnedResult.Value;
                    if (!success)
                    {
                        return Return(false);
                    }

                    ++frame.ChildIdx;

                    // 这里没法用循环，只能用 goto 了。
                    return GoTo(3);
                }
                default:
                {
                    throw new Exception("无效的程序计数器状态");
                }
            }
        }
    }
}
