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
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                context.Factory.SightingsImport.DetectNewLookups(context.Arguments[0]);
                context.Factory.SightingsImport.WriteNewLookupsToStream(context.Output);
                context.Output.Flush();
            }
        }
    }
}
