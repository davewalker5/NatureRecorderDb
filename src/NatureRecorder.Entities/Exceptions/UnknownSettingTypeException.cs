using System;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnknownSettingTypeException : Exception
    {
        public UnknownSettingTypeException()
        {
        }

        public UnknownSettingTypeException(string message) : base(message)
        {
        }

        public UnknownSettingTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
