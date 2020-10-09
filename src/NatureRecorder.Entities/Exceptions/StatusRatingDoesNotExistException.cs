using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusRatingDoesNotExistException : Exception
    {
        public StatusRatingDoesNotExistException()
        {
        }

        public StatusRatingDoesNotExistException(string message) : base(message)
        {
        }

        public StatusRatingDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}