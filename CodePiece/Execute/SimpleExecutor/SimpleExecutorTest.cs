using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Liu233w.Compiler.CodePiece.Execute.SimpleExecutor
{
    public class SimpleExecutorTest
    {
        [Fact]
        public void Test()
        {
            var input = new StreamReader(new MemoryStream(
                Encoding.ASCII.GetBytes(@"5
233")));
            var output = new MemoryStream();

            var program = new SimpleExecutor(new List<string[]>
            {
                new string[]
                {
                    "input"
                },
                new string[]
                {
                    "input"
                },
                new string[]
                {
                    "+"
                },
                new string[]
                {
                    "output"
                },
                new string[]
                {
                    "exit"
                }
            }, input, new StreamWriter(output));

            program.Execute();

            var s = Encoding.ASCII.GetString(output.ToArray());
            s.Should().Be("238");
        }
    }
}