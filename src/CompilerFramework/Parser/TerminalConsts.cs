namespace Liu233w.Compiler.CompilerFramework.Parser
{
    /// <summary>
    /// 某些终结符常量
    /// </summary>
    public static class TerminalConsts
    {
        /// <summary>
        /// 表示文件末尾
        /// </summary>
        public const string EndOfFile = null;

        /// <summary>
        /// 表示 ɛ
        /// </summary>
        public static string[] Epsilon { get; } = { };
    }
}