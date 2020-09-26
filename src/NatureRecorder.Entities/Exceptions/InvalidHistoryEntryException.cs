using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidHistoryEntryException : Exception
    {
        public InvalidHistoryEntryException()
        {
        }

        public InvalidHistoryEntryException(string message) : base(message)
        {
        }

        public InvalidHistoryEntryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
