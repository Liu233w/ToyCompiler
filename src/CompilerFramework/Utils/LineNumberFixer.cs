using System;
using System.Collections.Generic;
using System.Text;

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
        private readonly List<int> _lineMapper;

        private readonly string _buffer;

        public LineNumberFixer(string buffer)
        {
            _buffer = buffer;
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

        /// <summary>
        /// 获取一个字符串，用字符画的形式标出指定的位置
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetPositionMap(int line, int column)
        {
            var builder = GetMapLine(line);
            builder.Append(' ', column - 1);
            builder.Append('^');

            return builder.ToString();
        }

        /// <summary>
        /// 获取指定的行的 StringBuilder，最后一定有换行符。如果是最后一行（末尾没有换行符），会自动加上换行符
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private StringBuilder GetMapLine(int line)
        {
            var lastLine = line >= _lineMapper.Count;

            var startIdx = _lineMapper[line - 1];
            var endIdx = lastLine ? _buffer.Length - 1 : _lineMapper[line];

            var lineStr = _buffer.Substring(startIdx, endIdx - startIdx); // 包含了换行符

            var builder = new StringBuilder(lineStr);
            if (lastLine)
            {
                builder.AppendLine();
            }

            return builder;
        }

        /// <summary>
        /// 获取一个字符串，用字符画的形式标出指定的范围（只能标出一行中的范围）
        /// </summary>
        /// <param name="line"></param>
        /// <param name="begColumn"></param>
        /// <param name="endColumn"></param>
        /// <returns></returns>
        public string GetPositionRangeMap(int line, int begColumn, int endColumn)
        {
            var builder = GetMapLine(line);
            builder.Append(' ', begColumn - 1);
            builder.Append('|');
            builder.Append(' ', endColumn - begColumn - 1);
            builder.AppendLine();
            builder.Append(' ', begColumn - 1);
            builder.Append('-', endColumn - 1);

            return builder.ToString();
        }

        public (int line, int column, string positionMap) GetPositionWithMap(int index)
        {
            var (line, column) = GetPosition(index);
            var map = GetPositionMap(line, column);
            return (line, column, map);
        }
    }
}