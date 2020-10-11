using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StatusSchemeAlreadyExistsException : Exception
    {
        public StatusSchemeAlreadyExistsException()
        {
        }

        public StatusSchemeAlreadyExistsException(string message) : base(message)
        {
        }

        public StatusSchemeAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
