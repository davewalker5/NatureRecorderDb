using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CategoryDoesNotExistException : Exception
    {
        public CategoryDoesNotExistException()
        {
        }

        public CategoryDoesNotExistException(string message) : base(message)
        {
        }

        public CategoryDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
