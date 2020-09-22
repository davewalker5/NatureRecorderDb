using System.Collections.Generic;

namespace NatureRecorder.Interpreter.Interfaces
{
    public interface IInputReader
    {
        bool Cancelled { get; }
        char? PromptForOption(string prompt, IEnumerable<char> options);
        bool PromptForYesNo(string prompt);
        T Read<T>(string prompt);
        string ReadLine(string prompt, bool allowBlank);
    }
}