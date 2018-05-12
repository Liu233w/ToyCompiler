using System;
using System.Collections.Generic;
using System.Linq;

namespace LexicalAnalyzer
{
    /// <summary>
    /// 手动实现的栈帧虚拟机，能够使用 CallCC。目前只能处理一个函数的递归调用。
    /// </summary>
    /// <typeparam name="TParams">函数的参数，应该是一个POCO类，里面使用属性来存储函数参数。</typeparam>
    /// <typeparam name="TVariables">函数的内部变量。必须将变量储存在此处，否则在控制流转换时无法保存函数状态。格式参考 ParamTypes </typeparam>
    /// <typeparam name="TResult">函数的返回值类型</typeparam>
    public abstract class StackMachine<TParams, TVariables, TResult>
        where TParams : class, ICloneable
        where TVariables : class, ICloneable, new()
    {
        /// <summary>
        /// 一个函数的调用栈
        /// </summary>
        public class StackFrame
        {
            public TParams Params { get; set; }

            public TVariables Variables { get; set; }

            public TResult Result { get; set; }

            /// <summary>
            /// 程序计数器，表示函数的运行状态。如果是0，代表函数开头；-1代表函数已经运行结束，返回了值；其他值代表函数运行中的状态
            /// </summary>
            public int Pc { get; set; }

            /// <summary>
            /// call/cc 的返回结果，假如不是运行的 call/cc，是 null
            /// </summary>
            public object CallCCResult { get; set; }
        }

        /// <summary>
        /// 续延，保存了调用栈
        /// </summary>
        public class Continuation
        {
            internal Stack<StackFrame> Snapshot { get; set; }

            public Continuation(Stack<StackFrame> callingStack)
            {
                var snapshot = callingStack.Select(item => new StackFrame
                {
                    Pc = item.Pc,
                    Params = (TParams)item.Params.Clone(),
                    Variables = (TVariables)item.Variables.Clone(),
                    // Result = item.Result, // 不可能在返回之后调用 CallCC，所以这里不复制 Result
                });

                Snapshot = new Stack<StackFrame>(snapshot.Reverse());
            }
        }

        private Stack<StackFrame> _callingStack;

#if DEBUG
        private int _metaFunctionCalledTimes = 0;
#endif

        #region 函数中可以进行的操作

        /// <summary>
        /// 退出当前函数
        /// </summary>
        /// <param name="result">当前函数的返回值</param>
        protected bool Return(TResult result)
        {
            var frame = _callingStack.Peek();
            frame.Result = result;
            frame.Pc = -1;
#if DEBUG
            ++_metaFunctionCalledTimes;
#endif
            return false;
        }

        /// <summary>
        /// 步进 Pc，进入下一个状态
        /// </summary>
        /// <returns></returns>
        protected bool Continue()
        {
            ++_callingStack.Peek().Pc;
#if DEBUG
            ++_metaFunctionCalledTimes;
#endif
            return false;
        }

        /// <summary>
        /// 递归调用本函数，同时步进 Pc
        /// </summary>
        /// <param name="param">调用的参数</param>
        /// <returns></returns>
        protected bool RecursiveCall(TParams param)
        {
            ++_callingStack.Peek().Pc;
            _callingStack.Push(new StackFrame
            {
                Params = param,
                Variables = new TVariables(),
                Pc = 0,
            });
#if DEBUG
            ++_metaFunctionCalledTimes;
#endif
            return false;
        }

        /// <summary>
        /// 跳到某个状态
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        protected bool GoTo(int pc)
        {
            _callingStack.Peek().Pc = pc;
#if DEBUG
            ++_metaFunctionCalledTimes;
#endif
            return false;
        }

        /// <summary>
        /// 类似于 Scheme 的 call/cc，但是由于类型限制，这里的 call/cc 返回值是 Object 类型，需要自行转换
        /// </summary>
        /// <param name="lambda"></param>
        protected bool CallCC(Func<Continuation, object> lambda)
        {
            var result = lambda(new Continuation(_callingStack));
            ++_callingStack.Peek().Pc;

            _callingStack.Push(new StackFrame
            {
                Pc = -1,
                CallCCResult = result,
            });
#if DEBUG
            ++_metaFunctionCalledTimes;
#endif
            return false;
        }

        #endregion

        /// <summary>
        /// 从 Continuation 处重启运算，目前这个函数不能从内部运行
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public TResult ResumeAtContinuation(Continuation continuation, object callCCResult)
        {
            _callingStack = continuation.Snapshot;
            continuation.Snapshot = null;

            ++_callingStack.Peek().Pc;

            _callingStack.Push(new StackFrame
            {
                Pc = -1,
                CallCCResult = callCCResult,
            });

            return StartStepping();
        }

        /// <summary>
        /// 执行运算
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public TResult Run(TParams param)
        {
            _callingStack = new Stack<StackFrame>();
            _callingStack.Push(new StackFrame
            {
                Pc = 0,
                Params = param,
                Variables = new TVariables()
            });

            return StartStepping();
        }

        private TResult StartStepping()
        {
            // 调用栈未返回顶层
            while (_callingStack.Count > 1 || _callingStack.Peek().Pc != -1)
            {
                var frame = _callingStack.Peek();
                var lastResult = default(TResult);
                object callCCResult = null;

                if (frame.Pc == -1)
                {
                    // 退栈，保存返回值
                    lastResult = frame.Result;
                    callCCResult = frame.CallCCResult;
                    _callingStack.Pop();
                    frame = _callingStack.Peek();
                }

                StepMove(frame.Params, frame.Variables, frame.Pc, lastResult, callCCResult);
#if DEBUG
                if (_metaFunctionCalledTimes > 1)
                    throw new MustCallMetaFunctionWithReturnException();
                _metaFunctionCalledTimes = 0;
#endif
            }

            // 最后调用栈顶只剩下一个栈帧，保存了返回结果
            return _callingStack.Pop().Result;
        }

        /// <summary>
        /// 步进函数。复写此函数以实现逻辑。
        /// </summary>
        /// <param name="param">函数的输入</param>
        /// <param name="variables">函数的内部变量</param>
        /// <param name="pc">函数的程序计数器</param>
        /// <param name="lastReturned">递归调用的返回值，只有在上一次递归调用之后才有效。</param>
        /// <param name="callCCReturned">假如是从 call/cc 返回的，这里储存了 call/cc 的返回值</param>
        /// <returns>是否强制退栈（暂时没有用处）</returns>
        protected abstract bool StepMove(
            TParams param,
            TVariables variables,
            int pc,
            TResult lastReturned,
            object callCCReturned);
    }
}