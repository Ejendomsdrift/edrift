using System;
using System.Runtime.Serialization;

namespace Infrastructure.EventSourcing.Exceptions
{
    public class StorageUnavailableException : Exception
    {
        public StorageUnavailableException()
        {
        }

        public StorageUnavailableException(string message) : base(message)
        {
        }

        public StorageUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StorageUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}