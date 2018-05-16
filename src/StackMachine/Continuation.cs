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
            _callingStack = DeepCopyStack(callingStack);
        }

        /// <summary>
        /// 获取调用栈，将调用栈中的每个 stackMachine 设置成指定的对象
        /// </summary>
        /// <returns></returns>
        internal Stack<StackFrameFunction> GetCallingStack()
        {
            return DeepCopyStack(_callingStack);
        }

        private static Stack<StackFrameFunction> DeepCopyStack(Stack<StackFrameFunction> stack)
        {
            var snapshot = stack.Select(item => (StackFrameFunction)item.Clone());
            return new Stack<StackFrameFunction>(snapshot.Reverse());
        }
    }
}