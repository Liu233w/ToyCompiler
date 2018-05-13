using System;
using System.Collections.Generic;
using System.IO;
using Liu233w.Compiler.CompilerFramework.Tokenizer;
using Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions;
using Liu233w.Compiler.CompilerFramework.Utils;
using Liu233w.Compiler.EX1.libs;
using Shouldly;

namespace Liu233w.Compiler.EX1
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var buffer = File.ReadAllText(path);

            var res = Ex1Tokenizer.TokenizeBuffer(buffer);
            var fixer = new LineNumberFixer(buffer);

            res.Match(
                Right: r => PrintRight(r, fixer),
                Left: l => PrintLeft(l, fixer)
            );
        }

        private static void PrintRight(IEnumerable<Token> tokens, LineNumberFixer fixer)
        {
            foreach (var token in tokens)
            {
                var beginPosition = fixer.GetPosition(token.TokenBeginIdx).TidyPosition();
                var endPosition = fixer.GetPosition(token.TokenEndIdx).TidyPosition();

                Console.WriteLine($"类型：“{token.TokenType}”\n内容：“{token.Content}”\n" +
                                  $"起始位置：{beginPosition}\n终止位置：{endPosition}\n");
            }
        }

        private static void PrintLeft(IEnumerable<WrongTokenException> exceptions, LineNumberFixer fixer)
        {
            Console.Error.WriteLine("无法识别的语法单元：");
            Console.Error.WriteLine();

            foreach (var exception in exceptions)
            {
                var beginPosition = fixer.GetPosition(exception.TokenBegin);
                var endPosition = fixer.GetPosition(exception.CurrentIdx);
#if DEBUG
                beginPosition.line.ShouldBe(endPosition.line);
#endif
                Console.Error.WriteLine($"在 {endPosition.TidyPosition()} 处：");
                Console.Error.WriteLine();
                Console.Error.WriteLine(fixer.GetPositionRangeMap(beginPosition.line, beginPosition.column, endPosition.column));
                Console.Error.WriteLine();
            }
        }
    }
}
