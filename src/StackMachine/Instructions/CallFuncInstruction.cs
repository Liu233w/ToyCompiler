namespace Liu233w.StackMachine.Instructions
{
    /// <summary>
    /// 表示函数调用的指令
    /// </summary>
    public class CallFuncInstruction : FuncInstructionBase
    {
        public CallFuncInstruction(StackFrameFunction func)
        {
            Func = func;
        }

        public StackFrameFunction Func { get; }
    }
}