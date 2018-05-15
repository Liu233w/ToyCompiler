using System.Collections.Generic;

namespace Liu233w.Compiler.CodePiece.LexicalAnalyzer
{
    /// <summary>
    /// 用 LL(1) 算法检测数据是否符合定义
    /// </summary>
    public static class LlAnalyzer
    {
        /// <summary>
        /// 定义式： S ➡ (S)S | a
        /// </summary>
        /// <param name="input">要判断的句子</param>
        /// <returns></returns>
        public static bool Specific(string input)
        {
            var stack = new Stack<char>();
            stack.Push('S');

            var i = 0;
            while (stack.Count > 0)
            {
                if (i >= input.Length) return false;
                switch (stack.Peek())
                {
                    case 'S':
                    {
                        stack.Pop();
                        // 向前看一位
                        if (input[i] == '(')
                        {
                            foreach (var c in "S)S(")
                            {
                                stack.Push(c);
                            }
                        }
                        else
                        {
                            stack.Push('a');
                        }

                        break;
                    }

                    // 表中的其他条件

                    default: // 终止符
                    {
                        if (input[i++] != stack.Pop()) return false;

                        break;
                    }
                }
            }

            return i == input.Length;
        }
    }
}