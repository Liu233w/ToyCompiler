using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Liu233w.StackMachine.Test
{
    public class StackMachineTest
    {
        private StackMachine _machine;

        public StackMachineTest()
        {
            _machine = new StackMachine();
        }

        [Fact]
        public void 简单的函数()
        {
            var list = new List<int>();
            var result = _machine.Run(new SimpleFunc(0, list));
            var (r1, r2) = ((int, int))result;

            list.Should().BeEquivalentTo(new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            r1.Should().Be(0);
            r2.Should().Be(10);
        }

        [Fact]
        public void 测试CallCc()
        {
            var result = _machine.Run(new CallCcFunc());
            var (cont, returnedFromContinuation) = ((Continuation, bool)) result;

            returnedFromContinuation.Should().Be(false);

            result = _machine.RunWithContinuation(cont, true);
            (_, returnedFromContinuation) = ((Continuation, bool)) result;

            returnedFromContinuation.Should().Be(true);
        }
    }
}
