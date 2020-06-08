using System;
using NatureRecorder.Manager.Commands.Commands;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Nature Recorder Database Management {version}");

            CommandRunner runner = new CommandRunner();
            CommandParser parser = new CommandParser();

            try
            {
                parser.ParseCommandLine(args);

                if (parser.Command != null)
                {
                    runner.Run(parser.Command, parser.Arguments);
                }
                else
                {
                    runner.Run(new HelpCommand(), null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
