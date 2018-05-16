namespace Liu233w.StackMachine.Instructions
{
    /// <summary>
    /// 表示需要恢复续延的指令
    /// </summary>
    public class ResumeContinuationInstruction : FuncInstructionBase
    {
        public ResumeContinuationInstruction(Continuation cont, object input)
        {
            Cont = cont;
            Input = input;
        }

        public Continuation Cont { get; }

        public object Input { get; }
    }
}