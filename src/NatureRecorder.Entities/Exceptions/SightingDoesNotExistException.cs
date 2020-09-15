using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SightingDoesNotExistException : Exception
    {
        public SightingDoesNotExistException()
        {
        }

        public SightingDoesNotExistException(string message) : base(message)
        {
        }

        public SightingDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
