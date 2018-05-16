namespace Liu233w.StackMachine.Instructions
{
    /// <summary>
    /// 表示函数返回的指令
    /// </summary>
    public class ReturnInstruction : FuncInstructionBase
    {
        public ReturnInstruction(object result)
        {
            Result = result;
        }

        public object Result { get; }
    }
}