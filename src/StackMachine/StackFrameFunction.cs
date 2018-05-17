using System;
using Liu233w.StackMachine.Exceptions;
using Liu233w.StackMachine.Instructions;

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
        private int _pc;

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
            throw new MetaFuncWrongUsement("必须显式使用 base(0) 构造函数，或 base(that) 拷贝构造函数来初始化。不能使用默认构造函数");
        }

        protected StackFrameFunction(int _)
        {
            _pc = 0;
        }

        protected StackFrameFunction(StackFrameFunction that)
        {
            _pc = that._pc;
#if DEBUG
            _metaFunctionCalledTimes = that._metaFunctionCalledTimes;
#endif
        }

        /// <summary>
        /// 将函数步进一步
        /// </summary>
        /// <param name="lastReturned">假如上一步是函数调用或call/cc，这里应该是其返回值</param>
        /// <returns>经过此步之后函数对虚拟机发送的指令</returns>
        public FuncInstructionBase StepMove(object lastReturned)
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
        /// <returns>给虚拟机发送的指令</returns>
        protected abstract FuncInstructionBase StepMove(int pc, object lastReturned);

        #region StepMove函数中可以使用的操作

        /// <summary>
        /// 退出当前函数
        /// </summary>
        /// <param name="result">当前函数的返回值</param>
        protected FuncInstructionBase Return(object result)
        {
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return new ReturnInstruction(result);
        }

        /// <summary>
        /// 步进 Pc，进入下一个状态
        /// </summary>
        /// <returns></returns>
        protected FuncInstructionBase Continue()
        {
            ++_pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return NoneInstruction.Instance;
        }

        /// <summary>
        /// 调用下一个函数（必须也是 StackFrameFunction），并步进 PC
        /// </summary>
        /// <param name="func">调用的参数</param>
        /// <returns></returns>
        protected FuncInstructionBase CallStackFunc(StackFrameFunction func)
        {
            ++_pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return new CallFuncInstruction(func);
        }

        /// <summary>
        /// 跳到某个状态
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        protected FuncInstructionBase GoTo(int pc)
        {
            _pc = pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return NoneInstruction.Instance;
        }

        /// <summary>
        /// 类似于 Scheme 的 call/cc，但是由于类型限制，这里的 call/cc 返回值是 Object 类型，需要自行转换
        /// </summary>
        /// <param name="lambda"></param>
        protected FuncInstructionBase CallWithCurrentContinuation(Func<Continuation, object> lambda)
        {
            ++_pc;
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return new CallCcInstruction(lambda);
        }

        /// <summary>
        /// 恢复 Continuation，在这条指令后面的代码都不会执行
        /// </summary>
        /// <param name="cont"></param>
        /// <param name="input">续延的返回值</param>
        /// <returns></returns>
        protected FuncInstructionBase ResumeContinuation(Continuation cont, object input)
        {
#if DEBUG
            CheckMetaFunctionCalledTimes();
#endif
            return new ResumeContinuationInstruction(cont, input);
        }

        #endregion
    }
}