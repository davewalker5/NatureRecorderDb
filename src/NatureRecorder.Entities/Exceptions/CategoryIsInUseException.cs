using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CategoryIsInUseException : Exception
    {
        public CategoryIsInUseException()
        {
        }

        public CategoryIsInUseException(string message) : base(message)
        {
        }

        public CategoryIsInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
