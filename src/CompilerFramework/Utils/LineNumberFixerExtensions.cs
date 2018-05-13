namespace Liu233w.Compiler.CompilerFramework.Utils
{
    public static class LineNumberFixerExtensions
    {
        /// <summary>
        /// 将元组格式的位置转换成 1:2 这样的字符串形式
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string TidyPosition(this (int, int) position)
        {
            var (line, column) = position;
            return $"{line}:{column}";
        }
    }
}