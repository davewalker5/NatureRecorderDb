using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Logic
{
    [ExcludeFromCodeCoverage]
    public class CommandInterpreter
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
            new ExitCommand(),
            new ConnectionCommand(),
            new AddLocationCommand(),
            new AddCategoryCommand(),
            new AddSpeciesCommand(),
            new AddSightingCommand()
        };

        private static CommandInterpreter _instance = null;
        private static readonly object _lock = new object();
        private static readonly CommandRunner _runner = new CommandRunner(CommandMode.All);

        private CommandInterpreter()
        {
        }

        /// <summary>
        /// Retrieve an instance of the (singleton) command interpreter
        /// </summary>
        /// <param name="inMemoryContext"></param>
        /// <returns></returns>
        public static CommandInterpreter Instance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new CommandInterpreter();
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
                    _runner.Mode = CommandMode.CommandLine;
                    _runner.Run(command, arguments);
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
            // Set up the command runner
            _runner.Mode = CommandMode.Interactive;
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
                            _runner.Run(command, arguments);
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
