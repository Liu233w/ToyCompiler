using System;
using Liu233w.StackMachine.Exceptions;

namespace Liu233w.StackMachine
{
    /// <summary>
    /// 用在 <see cref="Liu233w.StackMachine.StackMachine"/> 中的函数。
    /// </summary>
    public abstract class StackFrameFunction : ICloneable
    {
        /// <summary>
        /// 当前函数的程序计数器
        /// </summary>
        internal int _pc;

        /// <summary>
        /// 跟函数相关联的栈帧虚拟机
        /// </summary>
        private StackMachine _stackMachine;

        /// <summary>
        /// 将本对象的虚拟机同指定的虚拟机关联起来
        /// </summary>
        /// <param name="stackMachine"></param>
        internal void AssignStackMachine(StackMachine stackMachine)
        {
            _stackMachine = stackMachine;
        }

#if DEBUG
        private int _metaFunctionCalledTimes;

        private void CheckMetaFunctionCalledTimes()
        {
            if (++_metaFunctionCalledTimes >= 2)
            {
                throw new MetaFuncWrongUsement("Meta 函数使用错误：在StepMove函数中必须将 Meta 函数同 return 连起来用\n" +
                                               "例如： return CallStackFunc(new ...Func(...));");
            }
        }
#endif

        protected StackFrameFunction()
        {
            _pc = 0;
            _stackMachine = null;
        }

        protected StackFrameFunction(StackFrameFunction that)
        {
            _pc = that._pc;
            _stackMachine = null;
#if DEBUG
            _metaFunctionCalledTimes = that._metaFunctionCalledTimes;
#endif
        }

        /// <summary>
        /// 步进函数的封装版本
        /// </summary>
        /// <param name="lastReturned"></param>
        /// <returns></returns>
        internal bool StepMove(object lastReturned)
        {
#if DEBUG
            _metaFunctionCalledTimes = 0;
#endif
            return StepMove(_pc, lastReturned);
        }

        public abstract object Clone();

        /// <summary>
        /// 步进函数。复写此函数以实现逻辑。
        /// </summary>
        /// <param name="pc">函数的程序计数器</param>
        /// <param name="lastReturned">上次调用的返回值，只有在调用之后才有效。</param>
        /// <returns>是否强制退栈（暂时没有用处）</returns>
        protected abstract bool StepMove(int pc, object lastReturned);

        #region StepMove函数中可以使用的操作

        /// <summary>
        /// 退出当前函数
        /// </summary>
        /// <param name="result">当前函数的返回值</param>
        protected bool Return(object result)
        {
            _stackMachine.FunctionReturn(result);
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return false;
        }

        /// <summary>
        /// 步进 Pc，进入下一个状态
        /// </summary>
        /// <returns></returns>
        protected bool Continue()
        {
            ++_pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return false;
        }

        /// <summary>
        /// 调用下一个函数（必须也是 StackFrameFunction），并步进 PC
        /// </summary>
        /// <param name="func">调用的参数</param>
        /// <returns></returns>
        protected bool CallStackFunc(StackFrameFunction func)
        {
            ++_pc;
            func._stackMachine = _stackMachine;
            _stackMachine.PushStack(func);
#if DEBUG
            CheckMetaFunctionCalledTimes();
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
            _pc = pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return false;
        }

        /// <summary>
        /// 类似于 Scheme 的 call/cc，但是由于类型限制，这里的 call/cc 返回值是 Object 类型，需要自行转换
        /// </summary>
        /// <param name="lambda"></param>
        protected bool CallWithCurrentContinuation(Func<Continuation, object> lambda)
        {
            ++_pc;
            var result = lambda(_stackMachine.GetCurrentContinuation());
            _stackMachine.ResumeWithResult(result);
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return false;
        }

        #endregion
    }
}