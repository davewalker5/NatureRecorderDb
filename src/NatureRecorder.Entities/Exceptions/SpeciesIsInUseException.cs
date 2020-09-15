using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SpeciesIsInUseException : Exception
    {
        public SpeciesIsInUseException()
        {
        }

        public SpeciesIsInUseException(string message) : base(message)
        {
        }

        public SpeciesIsInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
