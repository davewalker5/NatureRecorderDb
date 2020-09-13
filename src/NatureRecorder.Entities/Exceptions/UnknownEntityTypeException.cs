using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownEntityType : Exception
    {
        public UnknownEntityType()
        {
        }

        public UnknownEntityType(string message) : base(message)
        {
        }

        public UnknownEntityType(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
