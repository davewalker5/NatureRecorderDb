using System.Collections.Generic;

namespace NatureRecorder.Interpreter.Interfaces
{
    public interface IInputReader
    {
        bool Cancelled { get; }
        char? PromptForOption(string prompt, IEnumerable<char> options, char? defaultOption);
        bool PromptForYesNo(string prompt, char? defaultValue);
        T Read<T>(string prompt);
        string ReadLine(string prompt, bool allowBlank);
    }
}