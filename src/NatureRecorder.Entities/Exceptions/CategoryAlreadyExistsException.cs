using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CategoryAlreadyExistsException : Exception
    {
        public CategoryAlreadyExistsException()
        {
        }

        public CategoryAlreadyExistsException(string message) : base(message)
        {
        }

        public CategoryAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
