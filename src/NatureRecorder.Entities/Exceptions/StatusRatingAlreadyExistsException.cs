using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusRatingAlreadyExistsException : Exception
    {
        public StatusRatingAlreadyExistsException()
        {
        }

        public StatusRatingAlreadyExistsException(string message) : base(message)
        {
        }

        public StatusRatingAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}