using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Interpreter.Logic;

namespace NatureRecorder.Interpreter.Commands
{
    [ExcludeFromCodeCoverage]
    public class InteractiveShellCommand : CommandBase
    {
        public InteractiveShellCommand()
        {
            Type = CommandType.interactive;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.CommandLine;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                CommandInterpreter.Instance().RunInteractive();
            }
        }
    }
}
