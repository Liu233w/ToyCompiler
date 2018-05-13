using System;
using System.Collections.Generic;
using System.IO;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;
using Liu233w.Compiler.CompilerFramework.Utils;
using Liu233w.Compiler.EX1.libs;

namespace Liu233w.Compiler.EX1
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[1];
            var buffer = File.ReadAllText(path);

            var res = Ex1Tokenizer.TokenizeBuffer(buffer);
            var fixer = new LineNumberFixer(buffer);

        }

        private void PrintRight(List<Token> tokens, LineNumberFixer fixer)
        {
            foreach (var token in tokens)
            {
                var beginPosition = fixer.GetPosition(token.TokenBeginIdx).TidyPosition();
                var endPosition = fixer.GetPosition(token.TokenEndIdx).TidyPosition();

                Console.WriteLine($"类型：“{token.TokenType}”； 内容：“{token.Content}”；" +
                                  $"起始位置：{beginPosition}；终止位置：{endPosition} ");
            }
        }

        private void PrintLeft(List<WrongTokenException> exceptions, LineNumberFixer fixer)
        {

        }
    }
}
