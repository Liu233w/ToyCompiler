using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

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
        /// <summary>
        /// 一个栈帧。用于人工模拟的调用栈
        /// </summary>
        class CallingStackFrame
        { }

        /// <summary>
        /// 表示上个函数调用运行结果的栈帧
        /// </summary>
        class FunctionResultFrame : CallingStackFrame
        {
            public FunctionResultFrame(bool result)
            {
                Result = result;
            }

            public bool Result { get; private set; }
        }

        /// <summary>
        /// 表示函数参数的栈帧
        /// </summary>
        class FunctionParamFrame : CallingStackFrame
        {
            public FunctionParamFrame(int startIdx, Tree current)
            {
                StartIdx = startIdx;
                Current = current;
            }

            public int StartIdx { get; private set; }

            public Tree Current { get; private set; }

            /// <summary>
            /// 函数内部栈帧，用于存储函数内部分配的内存
            /// </summary>
            public (int ruleIdx, TreeNode tree, int childIdx) Inner { get; set; }
        }

        private List<Rule> _rules;
        private Stack<CallingStackFrame> _callingStack;
        private Stack<Stack<CallingStackFrame>> _resumeableCallingStack;
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

            _resumeableCallingStack = new Stack<Stack<CallingStackFrame>>();
        }

        /// <summary>
        /// 分析token序列，生成语法树（这里假设一个char是一个 token）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Tree Analyze(string buffer)
        {
            _buffer = buffer;
            ResetStack();

            // 起始节点
            var root = new TreeNode { Token = 'S' };
            _callingStack.Push(new FunctionParamFrame(0, root));

            // 未运算出结果
            while (_callingStack.Count > 1 || !(_callingStack.Peek() is FunctionResultFrame))
            {
                var state = AnalyzeNode();
                if (state.GetEnumerator().MoveNext())
                {
                    // 运行到了应当递归调用的地方
                }
                else
                {
                    
                }
            }

            var success = ((FunctionResultFrame) _callingStack.Pop()).Result;
            if (!success)
            {
                throw new InvalidOperationException("不是一个合格的语法");
            }

            return root;
        }

        /// <summary>
        /// 从模拟的调用栈运行分析函数。参数从调用栈中获取，返回值会被压入到调用栈中。
        /// </summary>
        /// <returns>
        /// 当前函数的控制流是否结束。
        /// 如果函数递归调用了下一个函数，会将其参数压入到调用栈中，并 yield return；
        /// 如果函数返回，会将其返回值压入到调用栈中，并 yield break。
        /// </returns>
        private IEnumerable AnalyzeNode()
        {
            var param = _callingStack.Peek() as FunctionParamFrame;
            Debug.Assert(param != null, nameof(param) + " != null");

            foreach (var rule in _rules)
            {
                if (_buffer[param.StartIdx] == rule.From)
                {
                    if (!(param.Current is TreeNode tree))
                    {
                        Return(false);

                        // 结束调用
                        yield break;
                    }

                    tree.Token = rule.From;
                    tree.Children = rule.To.Select(new Func<char, Tree>(c =>
                    {
                        if (char.IsUpper(c))
                        {
                            // 假设大写的都是非终结符号，小写的都是终结符号
                            return new TreeNode { Token = c };
                        }
                        else
                        {
                            return new TreeLeaf { Token = c };
                        }
                    })).ToList();

                    for (int i = 0; i < tree.Children.Count; ++i)
                    {
                        _callingStack.Push(new FunctionParamFrame(param.StartIdx + i + 1, tree.Children[i]));
                        // 挂起本函数
                        yield return 0;

                        var result = _callingStack.Pop() as FunctionResultFrame;
                        Debug.Assert(result != null, nameof(result) + " != null");
                        bool success = result.Result;

                        if (!success)
                        {
                            Return(false);
                            yield break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 退栈并将结果压入到栈中
        /// </summary>
        /// <param name="result"></param>
        private void Return(bool result)
        {
            // 移除函数参数
            _callingStack.Pop();
            // 压入函数结果
            _callingStack.Push(new FunctionResultFrame(result));
        }
    }
}
