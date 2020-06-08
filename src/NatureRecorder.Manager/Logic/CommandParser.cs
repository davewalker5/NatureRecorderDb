using System;
using System.Linq;
using NatureRecorder.Manager.Commands;
using NatureRecorder.Manager.Commands.Base;
using NatureRecorder.Manager.Commands.Commands;

namespace NatureRecorder.Manager.Logic
{
    public class CommandParser
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
            new ListSpeciesCommand()
        };

        public CommandBase Command { get; set; }
        public string[] Arguments { get; set; }

        /// <summary>
        /// Parse the command line, extracting the operation to be performed
        /// and its parameters 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public void ParseCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                // Attempt to parse out the operation type from the first argument
                if (Enum.TryParse<CommandType>(args[0], out CommandType type))
                {
                    // Find the first instance of a command of that type
                    Command = _commands.FirstOrDefault(c => c.Type == type);
                    if (Command != null)
                    {
                        // Extract the arguments, excluding the first which is the command name
                        Arguments = args.Skip(1).ToArray();
                    }
                }
            }
        }
    }
}
