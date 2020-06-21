using System.Collections.Generic;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ExportCommand : DataExchangeCommandBase
    {
        public ExportCommand()
        {
            Type = CommandType.export;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The third parameter is an arbitrary large number intended to capture all
                // sightings
                IEnumerable<Sighting> sightings = context.Factory.Sightings.List(null, 1, int.MaxValue);
                context.Factory.Export.RecordExport += OnRecordImportExport;
                context.Factory.Export.Export(sightings, context.Arguments[0]);
                context.Output.WriteLine($"\nExported the database to {context.Arguments[0]}");
                context.Output.Flush();
            }
        }
    }
}