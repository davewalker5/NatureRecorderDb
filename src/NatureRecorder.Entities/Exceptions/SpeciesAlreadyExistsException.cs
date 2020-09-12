using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SpeciesAlreadyExistsException : Exception
    {
        public SpeciesAlreadyExistsException()
        {
        }

        public SpeciesAlreadyExistsException(string message) : base(message)
        {
        }

        public SpeciesAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
