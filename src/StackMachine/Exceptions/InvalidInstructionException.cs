using System;
using System.Runtime.Serialization;

namespace Liu233w.StackMachine.Exceptions
{
    public class InvalidInstructionException : Exception
    {
        public InvalidInstructionException()
        {
        }

        public InvalidInstructionException(string message) : base(message)
        {
        }

        public InvalidInstructionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidInstructionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}