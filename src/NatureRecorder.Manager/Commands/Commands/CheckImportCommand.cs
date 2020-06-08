using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class CheckImportCommand : CommandBase
    {
        public CheckImportCommand()
        {
            Type = CommandType.check;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                factory.Import.DetectNewLookups(arguments[0]);
                factory.Import.WriteNewLookupsToConsole();
            }
        }
    }
}
