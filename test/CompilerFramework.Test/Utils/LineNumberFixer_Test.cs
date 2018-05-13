using System;
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
            buffer.Length.ShouldBe(16);
#endif

            _lineNumberFixer = new LineNumberFixer(buffer);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 1)]
        [InlineData(8, 2)]
        [InlineData(15, 2)]
        public void GetLineNumber_在奇数行时能够返回正确的行数(int idx, int line)
        {
            _lineNumberFixer.GetLineNumber(idx).ShouldBe(line);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(5, 0)]
        [InlineData(6, 1)]
        [InlineData(11, 1)]
        [InlineData(12, 2)]
        [InlineData(17, 2)]
        [InlineData(18, 3)]
        [InlineData(22, 3)]
        public void GetLineNumber_在偶数行时能够返回正确的行数(int idx, int line)
        {
            const string buffer = "12345\n67890\nqwert\nasdfg";
            buffer.Length.ShouldBe(23);

            new LineNumberFixer(buffer).GetLineNumber(idx).ShouldBe(line);
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 1, 2)]
        [InlineData(3, 1, 4)]
        [InlineData(4, 2, 1)]
        [InlineData(8, 3, 1)]
        [InlineData(15, 3, 8)]
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
            var map = _lineNumberFixer.GetPositionRangeMap(3, 3, 7);
            map.ShouldBe("ffffffff" + Environment.NewLine +
                         "  |   ^" + Environment.NewLine +
                         "  -----");
        }
    }
}