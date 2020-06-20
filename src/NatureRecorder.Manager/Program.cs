using System;
using NatureRecorder.Interpreter.Logic;

namespace NatureRecorder.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Version version = typeof(Program).Assembly.GetName().Version;
            Console.WriteLine($"Nature Recorder Database Management {version}");
            CommandInterpreter.Instance().RunCommandLine(args);
        }
    }
}
