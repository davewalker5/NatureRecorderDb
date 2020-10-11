using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusSchemeIsInUseException : Exception
    {
        public StatusSchemeIsInUseException()
        {
        }

        public StatusSchemeIsInUseException(string message) : base(message)
        {
        }

        public StatusSchemeIsInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

