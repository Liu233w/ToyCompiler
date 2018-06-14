using System;
using System.Runtime.Serialization;

namespace Liu233w.Compiler.EX2.Libs
{
    public class TokenNotMatchException : Exception
    {
        public TokenNotMatchException()
        {
        }

        public TokenNotMatchException(string message) : base(message)
        {
        }

        public TokenNotMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}