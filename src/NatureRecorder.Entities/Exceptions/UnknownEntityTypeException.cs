using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownEntityTypeException : Exception
    {
        public UnknownEntityTypeException()
        {
        }

        public UnknownEntityTypeException(string message) : base(message)
        {
        }

        public UnknownEntityTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
