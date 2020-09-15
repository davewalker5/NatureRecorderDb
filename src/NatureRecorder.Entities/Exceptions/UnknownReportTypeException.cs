using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownReportTypeException : Exception
    {
        public UnknownReportTypeException()
        {
        }

        public UnknownReportTypeException(string message) : base(message)
        {
        }

        public UnknownReportTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

