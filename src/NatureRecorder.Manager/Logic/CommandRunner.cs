using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Manager.Commands;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Logic
{
    public class CommandRunner
    {
        private readonly NatureRecorderFactory _factory;
        private readonly CommandContext _context;

        public CommandRunner(CommandContext commandContext)
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            _factory = new NatureRecorderFactory(context);
            _context = commandContext;
        }

        /// <summary>
        /// Run the specified command with the specified arguments
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        public void Run(CommandBase command, string[] arguments)
        {
            command.Run(_factory, _context, arguments);
        }
    }
}
