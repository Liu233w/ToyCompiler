using System;
using System.Collections.Generic;
using System.Diagnostics;
using Liu233w.Compiler.CompilerFramework.Parser.TreeNodes;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.StackMachine;
using Liu233w.StackMachine.Instructions;

namespace Liu233w.Compiler.CompilerFramework.Parser
{
    /// <summary>
    /// 使用 DFS + 回溯实现的语法分析器，可以分析任意BNF规则（除了左递归的以外）。
    /// 时间复杂度 O(n^3)，没法指出错误在哪里，而且可读性非常差。
    /// 跟 LL(1) 算法相比，唯一的优势是可以处理 S := ab|ac 这样的BNF。
    /// </summary>
    public static class DfsParser
    {
        /// <summary>
        /// 对Token序列进行语法分析
        /// </summary>
        /// <param name="tokens">要分析的序列</param>
        /// <param name="defination">用来分析的定义</param>
        /// <param name="startSymbol">起始项，一个在defination中出现的非终结符，来表示分析的起点，也是根节点</param>
        /// <param name="startIdx">从序列的哪个位置开始分析，默认是开头</param>
        /// <returns>如果序列符合定义，返回语法分析树；否则返回 null</returns>
        public static NonTerminalTree Parse(IList<Token> tokens, BnfDefination defination, string startSymbol,
            int startIdx = 0)
        {
            var machine = new StackMachine.StackMachine();
            var backtracing = new Stack<Continuation>();

            var (tree, scannedLength) = ((NonTerminalTree, int))
                machine.Run(new ParserFunc(startIdx, startSymbol, tokens, defination, backtracing));
            while (tree == null && backtracing.Count > 0)
            {
                var cont = backtracing.Pop();
                (tree, scannedLength) = ((NonTerminalTree, int))machine.RunWithContinuation(cont, true);
            }
            Assert(tree == null || scannedLength == tokens.Count, "在rule遍历时做了判断，这里一定相等");

            return tree;
        }

        #region 肮脏的内部实现

        private class ParserFunc : StackFrameFunction
        {

            // 函数参数
            private readonly int _startIdx; // 从第几个 token 开始处理
            private readonly Stack<Continuation> _backtracings;
            private readonly IList<Token> _tokens;
            private readonly BnfDefination _bnfDefination;
            private readonly string _currentNonTerminal;

            // 函数内部变量
            private int _currentTokenIdx;
            private int _ruleItemIdx; // 当前正在遍历 BNF 产生式的第几个 symbol
            private ISyntacticAnalysisTree[] _childs; // 当前非终结符的语法分析树的子节点
            private List<string[]> _ruleList; // 当前正在处理的symbol(非终结符)的规则列表
            private int _ruleIdx; // 上述列表的索引

            /// <summary>
            /// <see cref="StackFrameFunction"/> 函数的参数 
            /// </summary>
            /// <param name="startIdx">从token列表的哪个位置开始分析</param>
            /// <param name="currentNonTerminal">当前使用的BNF产生式，将使用这个式子检查接下来的token</param>
            /// <param name="tokens">Token列表</param>
            /// <param name="bnfDefination">所有的BNF产生式</param>
            /// <param name="backtracings">保存<see cref="Continuation"/>，用于回溯</param>
            public ParserFunc(int startIdx, string currentNonTerminal, IList<Token> tokens,
                BnfDefination bnfDefination, Stack<Continuation> backtracings)
                : base(0)
            {
                _startIdx = startIdx;
                _backtracings = backtracings;
                _tokens = tokens;
                _bnfDefination = bnfDefination;
                _currentNonTerminal = currentNonTerminal;
            }

            private ParserFunc(ParserFunc that)
                : base(that)
            {
                _startIdx = that._startIdx;
                // 浅拷贝，因为必须在全局保持状态一致
                _backtracings = that._backtracings;
                // 浅拷贝，因为是不变量
                _tokens = that._tokens;
                _bnfDefination = that._bnfDefination;

                _currentNonTerminal = that._currentNonTerminal;

                _currentTokenIdx = that._currentTokenIdx;
                _ruleItemIdx = that._ruleItemIdx;
                // 浅拷贝，因为回溯会覆盖之前的结果，不会产生影响
                _childs = that._childs;
                // 浅拷贝，因为是不变量
                _ruleList = that._ruleList;
                _ruleIdx = that._ruleIdx;
            }

            public override object Clone()
            {
                return new ParserFunc(this);
            }

            // 返回值： (NonTerminal, int) - Item1: 分析得到的语法树，如果是 null，代表分析失败；Item2: 下一个要扫描的 token
            protected override FuncInstructionBase StepMove(int pc, object lastReturned)
            {
                switch (pc)
                {
                    case 0:
                    {
                        if (_startIdx >= _tokens.Count)
                        {
                            return Return(((NonTerminalTree)null, _tokens.Count));
                        }

                        // 一开始一定是非终结符
                        _ruleList = _bnfDefination.GetRulesForNonTerminal(_currentNonTerminal);
                        _ruleIdx = 0;

                        return Continue();
                    }
                    case 1:
                    {
                        if (_ruleIdx >= _ruleList.Count)
                        {
                            // 没有找到合适的rule
                            return Return(((NonTerminalTree)null, _startIdx));
                        }

                        _childs = new ISyntacticAnalysisTree[_ruleList[_ruleIdx].Length];
                        _currentTokenIdx = _startIdx;
                        _ruleItemIdx = 0;

                        return Continue();
                    }
                    case 2:
                    {
                        if (_ruleItemIdx >= _ruleList[_ruleIdx].Length)
                        {
                            // child 全部填满，可以返回了
                            return GoTo(100);
                        }

                        // 是否到达末尾
                        if (_currentTokenIdx == _tokens.Count)
                        {
                            if (_ruleList[_ruleIdx][_ruleItemIdx] == TerminalConsts.EndOfFile)
                            {
                                // 在规则中应该是末尾
                                return GoTo(100);
                            }
                            else
                            {
                                // TODO: 这里有可能产生式是 ɛ，这样的话即使当前位置是末尾也可以满足要求
                                ++_ruleIdx;
                                return GoTo(1);
                            }
                        }

                        if (_ruleList[_ruleIdx][_ruleItemIdx] == TerminalConsts.EndOfFile)
                        {
                            // 在规则中应当到达末尾，但是目前没有到达末尾
                            ++_ruleIdx;
                            return GoTo(1);
                        }
                        else if (_bnfDefination.IsTerminal(_ruleList[_ruleIdx][_ruleItemIdx]))
                        {
                            // 根据规则，当前应该是终结符
                            var currentToken = _tokens[_currentTokenIdx];
                            if (currentToken.TokenType == _ruleList[_ruleIdx][_ruleItemIdx])
                            {
                                _childs[_ruleItemIdx] = new TerminalTree(currentToken);
                                ++_currentTokenIdx;
                                ++_ruleItemIdx;
                                return GoTo(2);
                            }
                            else
                            {
                                ++_ruleIdx;
                                return GoTo(1);
                            }
                        }
                        else
                        {
                            return Continue();
                        }
                    }
                    case 3:
                    {
                        // 当前的 ruleItem 是非终结符
                        return CallStackFunc(new ParserFunc(_currentTokenIdx, _ruleList[_ruleIdx][_ruleItemIdx],
                            _tokens, _bnfDefination, _backtracings));
                    }
                    case 4:
                    {
                        var (tree, nextIdx) = ((NonTerminalTree, int))lastReturned;
                        if (tree == null)
                        {
                            ++_ruleIdx;
                            return GoTo(1);
                        }
                        else
                        {
                            _childs[_ruleItemIdx] = tree;
                            _currentTokenIdx = nextIdx;
                            ++_ruleItemIdx;
                            return GoTo(2);
                        }
                    }
                    case 100:
                    {
                        return CallWithCurrentContinuation(cont =>
                        {
                            _backtracings.Push(cont);
                            return false;
                        });
                    }
                    case 101:
                    {
                        var shouldSkip = (bool)lastReturned;
                        if (shouldSkip)
                        {
                            ++_ruleIdx;
                            return GoTo(1);
                        }
                        else
                        {
                            return Return((
                                new NonTerminalTree(_currentNonTerminal, _childs, _ruleList[_ruleIdx]),
                                _currentTokenIdx
                                ));
                        }
                    }
                    default:
                    {
                        return null;
                    }
                }
            }
        }

        #endregion

        [Conditional("DEBUG")]
        private static void Assert(bool assertion, string message)
        {
            if (!assertion)
            {
                throw new Exception(message);
            }
        }

    }
}