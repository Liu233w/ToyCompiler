using System.Collections.Generic;

namespace Liu233w.StackMachine
{
    /// <summary>
    /// 栈帧虚拟机，用于运行 <see cref="StackFrameFunction"/>
    /// </summary>
    public class StackMachine
    {
        private Stack<StackFrameFunction> _callingStack = new Stack<StackFrameFunction>();

        /// <summary>
        /// 在虚拟机上运行函数，并返回结果。此操作将清空当前的调用栈。
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public object Run(StackFrameFunction func)
        {
            _callingStack.Clear();
            _callingStack.Push(func);

            StartStepping();

            return ((StackFrameResult)_callingStack.Pop()).Result;
        }

        /// <summary>
        /// 在当前的虚拟机上运行 Continuation
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public object RunWithContinuation(Continuation continuation)
        {
            _callingStack = continuation.GetAssignedCallingStack(this);
            StartStepping();
            return ((StackFrameResult)_callingStack.Pop()).Result;
        }

        /// <summary>
        /// 运行虚拟机，直到调用栈的顶部是结果为止
        /// </summary>
        private void StartStepping()
        {
            while (_callingStack.Count > 1 || !(_callingStack.Peek() is StackFrameResult))
            {
                object result = null;

                if (_callingStack.Peek() is StackFrameResult resultFrame)
                {
                    _callingStack.Pop();
                    result = resultFrame.Result;
                }

                _callingStack.Peek().StepMove(result);
            }
        }

        internal void PushStack(StackFrameFunction func)
        {
            _callingStack.Push(func);
        }

        /// <summary>
        /// 获取当前状态下的续延
        /// </summary>
        /// <returns></returns>
        internal Continuation GetCurrentContinuation()
        {
            return new Continuation(_callingStack);
        }

        /// <summary>
        /// 用一个栈帧来代替栈顶的函数栈帧
        /// </summary>
        /// <param name="result"></param>
        internal void FunctionReturn(object result)
        {
            _callingStack.Pop();
            ResumeWithResult(result);
        }

        /// <summary>
        /// 在栈顶放入一个结果
        /// </summary>
        /// <param name="result"></param>
        internal void ResumeWithResult(object result)
        {
            _callingStack.Push(new StackFrameResult(result));
        }
    }
}