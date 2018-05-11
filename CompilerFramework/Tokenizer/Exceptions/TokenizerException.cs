using System;
using System.Runtime.Serialization;

namespace Liu233w.Compiler.CompilerFramework.Tokenizer.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Tokenizer 相关的异常
    /// </summary>
    public class TokenizerException : Exception
    {
        public TokenizerException()
        {
        }

        public TokenizerException(string message) : base(message)
        {
        }

        public TokenizerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenizerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}