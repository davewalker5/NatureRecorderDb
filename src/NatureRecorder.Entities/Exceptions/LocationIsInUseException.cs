using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class LocationIsInUseException : Exception
    {
        public LocationIsInUseException()
        {
        }

        public LocationIsInUseException(string message) : base(message)
        {
        }

        public LocationIsInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
