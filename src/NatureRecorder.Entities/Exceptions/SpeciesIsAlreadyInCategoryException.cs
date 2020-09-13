using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SpeciesIsAlreadyInCategoryException : Exception
    {
        public SpeciesIsAlreadyInCategoryException()
        {
        }

        public SpeciesIsAlreadyInCategoryException(string message) : base(message)
        {
        }

        public SpeciesIsAlreadyInCategoryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
