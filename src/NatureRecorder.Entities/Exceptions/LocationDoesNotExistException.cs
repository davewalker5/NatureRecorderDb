using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class LocationDoesNotExistException : Exception
    {
        public LocationDoesNotExistException()
        {
        }

        public LocationDoesNotExistException(string message) : base(message)
        {
        }

        public LocationDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
