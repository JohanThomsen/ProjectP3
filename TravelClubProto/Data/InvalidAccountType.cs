using System;
using System.Runtime.Serialization;

namespace TravelClubProto.Data
{
    [Serializable]
    internal class InvalidAccountTypeException : Exception
    {
        public InvalidAccountTypeException()
        {
        }

        public InvalidAccountTypeException(string message) : base(message)
        {
        }

        public InvalidAccountTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAccountTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}