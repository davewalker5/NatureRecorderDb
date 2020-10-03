using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusSchemeDoesNotExistException : Exception
    {
        public StatusSchemeDoesNotExistException()
        {
        }

        public StatusSchemeDoesNotExistException(string message) : base(message)
        {
        }

        public StatusSchemeDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
