using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Logic
{
    [ExcludeFromCodeCoverage]
    public class CommandRunner
    {
        private readonly NatureRecorderFactory _factory;
        private readonly CommandMode _mode;

        public CommandRunner(CommandMode mode)
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            _factory = new NatureRecorderFactory(context);
            _mode = mode;
        }

        /// <summary>
        /// Run the specified command with the specified arguments
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        public void Run(CommandBase command, string[] arguments)
        {
            command.Run(new CommandContext
            {
                Reader = new ConsoleCommandReader(),
                Output = new StreamWriter(Console.OpenStandardOutput()),
                Factory = _factory,
                Mode = _mode,
                Arguments = arguments
            });
        }
    }
}
