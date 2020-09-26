using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnrecognisedHistoryActionException : Exception
    {
        public UnrecognisedHistoryActionException()
        {
        }

        public UnrecognisedHistoryActionException(string message) : base(message)
        {
        }

        public UnrecognisedHistoryActionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
