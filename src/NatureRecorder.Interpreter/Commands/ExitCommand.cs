using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    [ExcludeFromCodeCoverage]
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
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // Nothing to do
            }
        }
    }
}
