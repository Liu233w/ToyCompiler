using System;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// 在 Tokenizer 无法识别 Token 时抛出此异常
    /// </summary>
    public class WrongTokenException : TokenizerException
    {
        public string Buffer { get; set; }
        
        public int TokenBegin { get; set; }

        public int CurrentIdx { get; set; }

        public AutomataTokenizerState CurrentState { get; set; }

        public WrongTokenException(string message, string buffer, int tokenBegin, int currentIdx,
            AutomataTokenizerState currentState)
            : base(message)
        {
            CurrentIdx = currentIdx;
            CurrentState = currentState;
            Buffer = buffer;
            TokenBegin = tokenBegin;
        }
    }
}