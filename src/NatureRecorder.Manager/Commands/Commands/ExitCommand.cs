using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ExitCommand : CommandBase
    {
        public ExitCommand()
        {
            Type = CommandType.exit;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.Interactive;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // Nothing to do
            }
        }
    }
}
