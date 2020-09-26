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
        private const string Prompt = ">>";

        private CommandBase[] _commands = new CommandBase[]
        {
            new AddCommand(),
            new CheckImportCommand(),
            new ConnectionCommand(),
            new DeleteCommand(),
            new EditCommand(),
            new ExitCommand(),
            new ExportCommand(),
            new HelpCommand(),
            new HistoryCommand(),
            new ImportCommand(),
            new InteractiveShellCommand(),
            new ListCommand(),
            new MoveCommand(),
            new RenameCommand(),
            new ReportCommand(),
            new SetPasswordCommand(),
            new UpdateDatabaseCommand()
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

                    // If the command comes back with a recalled command associated
                    // with it, run that as this has been recalled from the command
                    // history
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
            _runner.History = new CommandHistory();
            bool exit = false;

            // Show the current database connection
            _runner.Run(_commands.First(c => c.Type == CommandType.connection), new string[] { });

            do
            {
                // Read the next command
                Console.Write($"{Prompt} ");
                string commandLine = Console.ReadLine().Trim();
                while (!string.IsNullOrEmpty(commandLine))
                {
                    // Perform command shortcut replacements
                    commandLine = ReplaceCommandShortcuts(commandLine);

                    // Split the command text into an arguments array and run it
                    string[] args = SplitCommandLine(commandLine);
                    (CommandBase command, string[] arguments) = IdentifyCommand(args);
                    if (command != null)
                    {
                        // If the command isn't one of the "history" command variants
                        // or the exit command, add it to the history
                        if ((command.Type != CommandType.history) && (command.Type != CommandType.exit))
                        {
                            _runner.History.Add(commandLine);
                        }

                        try
                        {
                            // Run the command and end if this was the exit command
                            exit = command.Type == CommandType.exit;
                            commandLine = _runner.Run(command, arguments);

                            // If we have a recalled command, display and run it
                            if (!string.IsNullOrEmpty(commandLine))
                            {
                                Console.WriteLine($"{Prompt} {commandLine}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        _runner.History.Add(commandLine);
                        Console.WriteLine("Error: Invalid command or arguments");

                        // Must clear the command line, now, or we'll loop continuously.
                        // There is no recalled command after an error
                        commandLine = null;
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

        /// <summary>
        /// Perform command shortcut replacements
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        private string ReplaceCommandShortcuts(string commandLine)
        {
            // Currently, there's only one command shortcut - "!" as the first
            // character is short for the "history" command but *only* if it's
            // followed by more text, which is assumed at this point to be the
            // entry number to recall
            string replaced = commandLine;

            // By this point, we can rely on the command line already having been
            // trimmed so [0] is the first non-whitespace character
            if ((replaced[0] == '!') && (replaced.Length > 1))
            {
                replaced = $"{CommandType.history.ToString()} {replaced.Substring(1)}";
            }

            return replaced;
        }
    }
}
