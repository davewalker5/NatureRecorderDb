using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class EntityTypeNotValidForRenameException : Exception
    {
        public EntityTypeNotValidForRenameException()
        {
        }

        public EntityTypeNotValidForRenameException(string message) : base(message)
        {
        }

        public EntityTypeNotValidForRenameException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
