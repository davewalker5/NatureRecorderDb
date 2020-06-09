using System;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Nature Recorder Database Management {version}");
            Interpreter.Instance().RunCommandLine(args);
        }
    }
}
