using System;
using System.Collections.Generic;
using System.IO;

namespace Liu233w.Compiler.CodePiece.Execute.SimpleExecutor
{
    public static class TestSimpleCalculator
    {
        public static void Exec()
        {
            new SimpleExecutor(new List<string[]>
                {
                    new[] {"load", "Welcome To Calculator\n"},
                    new[] {"output"},
                    new[] {"load", "Enter the first number: "},
                    new[] {"output"},
                    new[] {"input"},
                    new[] {"load", "Enter the second number: "},
                    new[] {"output"},
                    new[] {"input"},
                    new[] {"+"},
                    new[] {"load", "Add a and b, the result is: "},
                    new[] {"output"},
                    new[] {"output"},
                    new[] {"load", "\n"},
                    new[] {"output"},
                    new[] {"load", "Continue? [1/0]: "},
                    new[] {"output"},
                    new[] {"input"},
                    new[] {"if-false-jmp", "19"},
                    new[] {"jmp", "2"},
                    new[] {"exit"},
                }, new StreamReader(Console.OpenStandardInput()), new StreamWriter(Console.OpenStandardOutput()))
                .Execute();
        }
    }
}