using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class InteractiveShellCommand : CommandBase
    {
        public InteractiveShellCommand()
        {
            Type = CommandType.interactive;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.CommandLine;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                Interpreter.Instance().RunInteractive();
            }
        }
    }
}
