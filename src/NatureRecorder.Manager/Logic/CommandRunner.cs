using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Manager.Commands;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Logic
{
    public class CommandRunner
    {
        private readonly NatureRecorderFactory _factory;

        public CommandRunner()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            _factory = new NatureRecorderFactory(context);
        }

        /// <summary>
        /// Run the specified command with the specified arguments
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        public void Run(CommandBase command, string[] arguments)
        {
            command.Run(_factory, CommandContext.CommandLine, arguments);
        }
    }
}
