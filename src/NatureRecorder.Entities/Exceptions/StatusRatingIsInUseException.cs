using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusRatingIsInUseException : Exception
    {
        public StatusRatingIsInUseException()
        {
        }

        public StatusRatingIsInUseException(string message) : base(message)
        {
        }

        public StatusRatingIsInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
