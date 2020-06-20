using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class CheckImportCommand : CommandBase
    {
        public CheckImportCommand()
        {
            Type = CommandType.check;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Import.DetectNewLookups(context.Arguments[0]);
                context.Factory.Import.WriteNewLookupsToStream(context.Output);
                context.Output.Flush();
            }
        }
    }
}
