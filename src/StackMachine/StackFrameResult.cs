using Liu233w.StackMachine.Instructions;

namespace Liu233w.StackMachine
{
    /// <summary>
    /// 用于表示返回值的类，在函数退栈之后，会使用当前类的对象来代替
    /// </summary>
    internal class StackFrameResult : StackFrameFunction
    {
        internal object Result { get; private set; }

        internal StackFrameResult(object result)
        {
            Result = result;
        }

        private StackFrameResult(StackFrameResult that)
            : base(that)
        {
        }

        public override object Clone()
        {
            return new StackFrameResult(this);
        }

        protected override FuncInstructionBase StepMove(int pc, object lastReturned)
        {
            throw new System.NotImplementedException("不应该再调用此函数");
        }
    }
}