using System;
using System.Diagnostics;
using Liu233w.Compiler.CompilerFramework.Utils;
using Shouldly;
using Xunit;

namespace Liu233w.Compiler.CompilerFramework.Test.Utils
{
    // ReSharper disable once InconsistentNaming
    public class LineNumberFixer_Test
    {
        private readonly LineNumberFixer _lineNumberFixer;

        public LineNumberFixer_Test()
        {
            // 最后一行可能没有换行符
            const string buffer = "123\n456\nffffffff";
#if DEBUG
            buffer.Length.ShouldBe(17);
#endif

            _lineNumberFixer = new LineNumberFixer(buffer);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 1)]
        [InlineData(8, 2)]
        [InlineData(16, 2)]
        public void GetLineNumber_能够返回正确的行数(int idx, int line)
        {
            _lineNumberFixer.GetLineNumber(idx).ShouldBe(line);
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 1, 2)]
        [InlineData(3, 1, 4)]
        [InlineData(4, 2, 1)]
        [InlineData(8, 3, 1)]
        [InlineData(16, 3, 9)]
        public void GetPosition_能够返回正确的位置(int idx, int expectLine, int expectColumn)
        {
            var (line, column) = _lineNumberFixer.GetPosition(idx);
            line.ShouldBe(expectLine);
            column.ShouldBe(expectColumn);
        }

        [Fact]
        public void GetPositionMap_能够获得正确的Map()
        {
            var map = _lineNumberFixer.GetPositionMap(2, 2);
            map.ShouldBe("456\n" +
                         " ^");
        }

        [Fact]
        public void GetPositionRangeMap_能够获得正确的Map()
        {
            var map = _lineNumberFixer.GetPositionRangeMap(3, 2, 6);
            map.ShouldBe("ffffffff" + Environment.NewLine +
                         " |   ^" + Environment.NewLine +
                         " -----");
        }
    }
}