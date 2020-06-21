using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    [ExcludeFromCodeCoverage]
    public class UpdateDatabaseCommand : CommandBase
    {
        public UpdateDatabaseCommand()
        {
            Type = CommandType.update;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Context.Database.Migrate();
                context.Output.WriteLine($"Applied the latest database migrations");
                context.Output.Flush();
            }
        }
    }
}
