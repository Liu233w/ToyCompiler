using System.Collections.Generic;
using System.Linq;

namespace Liu233w.Compiler.CodePiece.Parser
{
    // ReSharper disable once InconsistentNaming
    public static class LRParser
    {
        private static (char, string)[] _defination =
        {
            ('S', "aAcBe"),
            ('A', "b"),
            ('A', "Ab"),
            ('B', "d"),
        };

        public static bool Parse(string input, int idx = 0)
        {
            var current = "";

            while (idx < input.Length)
            {
                if (FindDefination(current, out var defination))
                {
                    current.RemoveLastLength(defination.Item2.Length);
                    current += defination.Item1;
                }
                else
                {
                    current += input[idx++];
                }
            }

            while (FindDefination(current, out var defination))
            {
                current.RemoveLastLength(defination.Item2.Length);
                current += defination.Item1;
            }

            return current == "S";
        }

        public static bool FindDefination(string current, out (char, string) defination)
        {
            foreach (var item in _defination)
            {
                if (current.EndsWith(item.Item2))
                {
                    defination = item;
                    return true;
                }
            }

            defination = ('\0', null);
            return false;
        }

        /// <summary>
        /// 移除字符串的后n个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns>返回被移除的字符</returns>
        public static string RemoveLastLength(this string str, int length)
        {
            return str.Remove(str.Length - length);
        }
    }
}