using System;
using NatureRecorder.Manager.Entities;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Nature Recorder Database Management {version}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                new CommandRunner().Run(op);
            }
            else
            {
                op = new Operation { Type = OperationType.help };
                new CommandRunner().Run(op);
            }
        }
    }
}
