using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownDataExchangeTypeException : Exception
    {
        public UnknownDataExchangeTypeException()
        {
        }

        public UnknownDataExchangeTypeException(string message) : base(message)
        {
        }

        public UnknownDataExchangeTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

