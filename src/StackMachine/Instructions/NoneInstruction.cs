namespace Liu233w.StackMachine.Instructions
{
    /// <summary>
    /// 表示没有指令
    /// </summary>
    public class NoneInstruction : FuncInstructionBase
    {
        private NoneInstruction()
        {
        }

        private static NoneInstruction _instance;

        public static NoneInstruction Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NoneInstruction();
                }

                return _instance;
            }
        }
    }
}