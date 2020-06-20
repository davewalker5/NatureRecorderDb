using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ExitCommand : CommandBase
    {
        public ExitCommand()
        {
            Type = CommandType.exit;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                // Nothing to do
            }
        }
    }
}
