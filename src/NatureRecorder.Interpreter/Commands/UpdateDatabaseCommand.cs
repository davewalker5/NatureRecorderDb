using System;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
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
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Context.Database.Migrate();
                context.Output.WriteLine($"Applied the latest database migrations");
                context.Output.Flush();
            }
        }
    }
}
