using System;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class UpdateDatabaseCommand : CommandBase
    {
        public UpdateDatabaseCommand()
        {
            Type = CommandType.update;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                factory.Context.Database.Migrate();
                Console.WriteLine($"Applied the latest database migrations");
            }
        }
    }
}
