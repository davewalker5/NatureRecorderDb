using System;
using System.Linq;
using NatureRecorder.Manager.Commands;
using NatureRecorder.Manager.Commands.Base;
using NatureRecorder.Manager.Commands.Commands;

namespace NatureRecorder.Manager.Logic
{
    public sealed class Interpreter
    {
        private CommandBase[] _commands = new CommandBase[]
        {
            new AddUserCommand(),
            new SetPasswordCommand(),
            new DeleteUserCommand(),
            new ExportCommand(),
            new CheckImportCommand(),
            new ImportCommand(),
            new SummaryCommand(),
            new ReportCommand(),
            new UpdateDatabaseCommand(),
            new HelpCommand(),
            new ListLocationsCommand(),
            new ListCategoriesCommand(),
            new ListSpeciesCommand(),
            new InteractiveShellCommand(),
            new ExitCommand()
        };

        private static Interpreter _instance = null;
        private static readonly object _lock = new object();

        private Interpreter()
        {
        }

        /// <summary>
        /// Retrieve an instance of the (singleton) command interpreter
        /// </summary>
        /// <returns></returns>
        public static Interpreter Instance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Interpreter();
                }
            }

            return _instance;
        }

        /// <summary>
        /// Run the command specified in the arguments array in command line
        /// mode
        /// </summary>
        /// <param name="args"></param>
        public void RunCommandLine(string[] args)
        {
            // Identify the command and separate out its arguments
            (CommandBase command, string[] arguments) = IdentifyCommand(args);
            if (command != null)
            {
                try
                {
                    // Run the command
                    CommandRunner runner = new CommandRunner(CommandContext.CommandLine);
                    runner.Run(command, arguments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Error: Invalid command or arguments");
            }
        }

        /// <summary>
        /// Run the interactive shell
        /// </summary>
        public void RunInteractive()
        {
            // Create a command runner 
            CommandRunner runner = new CommandRunner(CommandContext.Interactive);
            bool exit = false;

            do
            {
                // Read the next command
                Console.Write(">> ");
                string commandLine = Console.ReadLine().Trim();
                if (!string.IsNullOrEmpty(commandLine))
                {
                    // Split the command text into an arguments array and run it
                    string[] args = SplitCommandLine(commandLine);
                    (CommandBase command, string[] arguments) = IdentifyCommand(args);
                    if (command != null)
                    {
                        try
                        {
                            // Run the command and end if this was the exit command
                            exit = command.Type == CommandType.exit;
                            runner.Run(command, arguments);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid command or arguments");
                    }

                }
            }
            while (!exit);
        }

        /// <summary>
        /// Identify the command, extracting the operation to be performed
        /// and its parameters
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private (CommandBase command, string[] arguments) IdentifyCommand(string[] args)
        {
            CommandBase command = null;
            string[] arguments = null;

            if ((args != null) && (args.Length > 0))
            {
                // Attempt to parse out the operation type from the first argument
                if (Enum.TryParse<CommandType>(args[0], out CommandType type))
                {
                    // Find the first instance of a command of that type
                    command = _commands.FirstOrDefault(c => c.Type == type);
                    if ((command != null) && (args != null))
                    {
                        // Extract the arguments, excluding the first which is the command name
                        arguments = args.Skip(1).ToArray();
                    }
                }
            }

            return (command, arguments);
        }

        /// <summary>
        /// Split a string containing a command and parameters into a command-line
        /// style args[] array
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        private string[] SplitCommandLine(string commandLine)
        {
            // This implementation is taken from the following Stack Overflow page:
            // https://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp/298990#298990

            bool inQuotes = false;

            return commandLine.Split(c =>
                                    {
                                        if (c == '\"')
                                            inQuotes = !inQuotes;

                                        return !inQuotes && c == ' ';
                                    })
                                    .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                                    .Where(arg => !string.IsNullOrEmpty(arg))
                                    .ToArray();
        }
    }
}
