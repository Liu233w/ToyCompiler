using System;
using System.Collections.Generic;

namespace Liu233w.Compiler.CompilerFramework.Utils
{
    /// <summary>
    /// 输入一个 buffer 中的位置，输出它在 buffer 中的第几行，第几列。
    /// 能根据 \n 和 \r\n 分割行数
    /// </summary>
    public class LineNumberFixer
    {
        /// <summary>
        /// [x] = y 表示第x行第一个字符是 buffer 中的第y个字符
        /// </summary>
        private List<int> _lineMapper;

        public LineNumberFixer(string buffer)
        {
            _lineMapper = new List<int> { 0 };

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == '\n')
                {
                    _lineMapper.Add(i + 1);
                }
                else if (buffer[i] == '\r')
                {
                    if (buffer[i + 1] != '\n') continue;

                    ++i;
                    _lineMapper.Add(i + 1);
                }
            }
        }

        /// <summary>
        /// 获取 index 在 buffer 的第几行，行数从 0 开始
        /// </summary>
        /// <param name="index">buffer中的绝对位置</param>
        /// <returns></returns>
        public int GetLineNumber(int index)
        {
            var beg = 0;
            var end = _lineMapper.Count - 1;

            while (beg < end - 1)
            {
                if (_lineMapper[beg] == index) return beg;
                if (_lineMapper[end] == index) return end;

                var middle = (end + beg) / 2;
                var middleVal = _lineMapper[middle];
                if (middleVal < index)
                    beg = middle;
                else if (middleVal > index)
                    end = middle;
                else
                    return middle;
            }

            // 最后肯定在 beg 和 end 之间
            return beg;
        }

        /// <summary>
        /// 获取idx在buffer中的行号和列号，序号从 1 开始
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int line, int column) GetPosition(int index)
        {
            var line = GetLineNumber(index);
            return (line + 1, index - _lineMapper[line] + 1);
        }
    }
}