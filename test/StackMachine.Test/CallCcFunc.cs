using Liu233w.StackMachine.Instructions;

namespace Liu233w.StackMachine.Test
{
    public class CallCcFunc : StackFrameFunction
    {
        private Continuation _cont;

        public CallCcFunc()
        {
        }

        public CallCcFunc(CallCcFunc that)
            : base(that)
        {
            _cont = that._cont;
        }

        public override object Clone()
        {
            return new CallCcFunc(this);
        }

        protected override FuncInstructionBase StepMove(int pc, object lastReturned)
        {
            switch (pc)
            {
                case 0:
                {
                    return CallWithCurrentContinuation(cont =>
                    {
                        _cont = cont;
                        return false;
                    });
                }
                case 1:
                {
                    // 这里必须转换 lastReturned，否则之后就没法转换了
                    return Return((_cont, (bool)lastReturned));
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}