using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownExportTypeException : Exception
    {
        public UnknownExportTypeException()
        {
        }

        public UnknownExportTypeException(string message) : base(message)
        {
        }

        public UnknownExportTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

