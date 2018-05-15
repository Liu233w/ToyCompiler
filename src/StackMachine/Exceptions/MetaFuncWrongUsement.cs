using System;
using System.Runtime.Serialization;

namespace Liu233w.StackMachine.Exceptions
{
    /// <summary>
    /// 在元函数使用错误时抛出此异常
    /// </summary>
    public class MetaFuncWrongUsement : Exception
    {
        public MetaFuncWrongUsement()
        {
        }

        public MetaFuncWrongUsement(string message) : base(message)
        {
        }

        public MetaFuncWrongUsement(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MetaFuncWrongUsement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}