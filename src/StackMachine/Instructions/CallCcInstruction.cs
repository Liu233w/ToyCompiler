using System;

namespace Liu233w.StackMachine.Instructions
{
    /// <summary>
    /// 表示一个 call/cc 的指令
    /// </summary>
    public class CallCcInstruction : FuncInstructionBase
    {
        public CallCcInstruction(Func<Continuation, object> lambda)
        {
            Lambda = lambda;
        }

        public Func<Continuation, object> Lambda { get; }
    }
}