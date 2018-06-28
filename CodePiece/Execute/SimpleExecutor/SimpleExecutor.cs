using System;
using System.Collections.Generic;
using System.IO;

namespace Liu233w.Compiler.CodePiece.Execute.SimpleExecutor
{
    /// <summary>
    /// 简单的字节码解释器，没有函数调用功能，基于栈。只有加法运算，不图灵完备。
    /// </summary>
    public class SimpleExecutor
    {
        private readonly List<string[]> _commands;

        private readonly Stack<string> _stack;

        private int _pc;

        private StreamReader _reader;

        private StreamWriter _writer;

        public SimpleExecutor(List<string[]> commands, StreamReader reader, StreamWriter writer)
        {
            _commands = commands;
            _reader = reader;
            _writer = writer;
            _stack = new Stack<string>();
            _pc = 0;
        }

        public string Execute()
        {
            while (true)
            {
                var cmd = _commands[_pc];
                switch (cmd[0])
                {
                    case "return":
                    {
                        return _stack.Pop();
                    }
                    case "exit":
                    {
                        return null;
                    }
                    case "jmp":
                    {
                        _pc = int.Parse(cmd[1]);
                        break;
                    }
                    case "jmps":
                    {
                        var p = _stack.Pop();
                        _pc = int.Parse(p);
                        break;
                    }
                    case "if-false-jmp":
                    {
                        var assert = _stack.Pop();
                        if (int.TryParse(assert, out var res) && res == 0)
                        {
                            _pc = int.Parse(cmd[1]);
                        }
                        else
                        {
                            ++_pc;
                        }

                        break;
                    }
                    case "+":
                    {
                        var a = _stack.Pop();
                        var b = _stack.Pop();
                        _stack.Push((int.Parse(a) + int.Parse(b)).ToString());
                        ++_pc;
                        break;
                    }
                    case "load":
                    {
                        _stack.Push(cmd[1]);
                        ++_pc;
                        break;
                    }
                    case "input":
                    {
                        var input = _reader.ReadLine();
                        _stack.Push(input);
                        ++_pc;
                        break;
                    }
                    case "output":
                    {
                        _writer.Write(_stack.Pop());
                        _writer.Flush();
                        ++_pc;
                        break;
                    }
                    default:
                        throw new Exception("Unhandled command");
                }
            }
        }
    }
}