﻿namespace NatureRecorder.Interpreter.Interfaces
{
    public interface IInputReader
    {
        bool Cancelled { get; }
        bool PromptForYesNo(string prompt);
        T Read<T>(string prompt);
        string ReadLine(string prompt, bool allowBlank);
    }
}