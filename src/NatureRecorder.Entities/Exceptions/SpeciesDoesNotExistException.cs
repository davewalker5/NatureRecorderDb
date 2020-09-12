using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SpeciesDoesNotExistException : Exception
    {
        public SpeciesDoesNotExistException()
        {
        }

        public SpeciesDoesNotExistException(string message) : base(message)
        {
        }

        public SpeciesDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}