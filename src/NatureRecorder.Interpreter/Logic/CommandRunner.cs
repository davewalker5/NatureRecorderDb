using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Logic
{
    [ExcludeFromCodeCoverage]
    public class CommandRunner
    {
        private readonly NatureRecorderFactory _factory;
        private Location _currentLocation = null;
        private DateTime? _currentDate = null;

        public CommandMode Mode { get; set; }
        public CommandHistory History { get; set; }
        public UserSettings Settings { get; set; }

        public CommandRunner(CommandMode mode)
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            _factory = new NatureRecorderFactory(context);
            Mode = mode;
        }

        /// <summary>
        /// Run the specified command with the specified arguments
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string Run(CommandBase command, string[] arguments)
        {
            // Create a context for running the command. The current date and
            // location are used to provide defaults during data entry, so the
            // user doesn't have to repeatedly enter them when entering a batch
            // of sightings
            CommandContext context = new CommandContext
            {
                Reader = new ConsoleCommandReader(),
                Output = new StreamWriter(Console.OpenStandardOutput()),
                Factory = _factory,
                Mode = Mode,
                Arguments = arguments,
                CurrentLocation = _currentLocation,
                CurrentDate = _currentDate,
                History = History,
                Settings = Settings
            };

            // Run the command and capture the current date and location, that
            // may have been updated
            command.Run(context);
            _currentDate = context.CurrentDate;
            _currentLocation = context.CurrentLocation;

            // If a command's been recalled from history, return it so it can
            // also be run
            return context.RecalledCommand;
        }
    }
}
