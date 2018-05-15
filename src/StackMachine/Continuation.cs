using System.Collections.Generic;
using System.Linq;

namespace Liu233w.StackMachine
{
    /// <summary>
    /// 程序的续延，需要配合 <see cref="StackMachine"/> 使用
    /// </summary>
    public class Continuation
    {
        private Stack<StackFrameFunction> _callingStack;

        internal Continuation(Stack<StackFrameFunction> callingStack)
        {
            var snapshot = callingStack.Select(item => (StackFrameFunction)item.Clone());
            _callingStack = new Stack<StackFrameFunction>(snapshot.Reverse());
        }

        /// <summary>
        /// 获取调用栈，将调用栈中的每个 stackMachine 设置成指定的对象
        /// </summary>
        /// <returns></returns>
        internal Stack<StackFrameFunction> GetAssignedCallingStack(StackMachine stackMachine)
        {
            var snapshot = _callingStack.Select(item =>
            {
                var res = (StackFrameFunction)item.Clone();
                res.AssignStackMachine(stackMachine);
                return res;
            });
            return new Stack<StackFrameFunction>(snapshot.Reverse());
        }
    }
}