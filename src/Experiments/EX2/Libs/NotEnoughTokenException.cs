using System;
using System.Runtime.Serialization;

namespace Liu233w.Compiler.EX2.Libs
{
    /// <summary>
    /// 表示token数量不够的异常
    /// </summary>
    public class NotEnoughTokenException : Exception
    {
        public NotEnoughTokenException()
        {
        }

        public NotEnoughTokenException(string message) : base(message)
        {
        }

        public NotEnoughTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotEnoughTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}