using System;
using System.Runtime.Serialization;

namespace Liu233w.Compiler.EX2.Libs
{
    /// <summary>
    /// 表示语义错误的异常
    /// </summary>
    public class WrongSemanticException : Exception
    {
        public WrongSemanticException()
        {
        }

        public WrongSemanticException(string message) : base(message)
        {
        }

        public WrongSemanticException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongSemanticException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}