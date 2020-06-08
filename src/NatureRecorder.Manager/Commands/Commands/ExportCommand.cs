using System;
using System.Collections.Generic;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ExportCommand : DataExchangeCommandBase
    {
        public ExportCommand()
        {
            Type = CommandType.export;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // The third parameter is an arbitrary large number intended to capture all
                // sightings
                IEnumerable<Sighting> sightings = factory.Sightings.List(null, 1, int.MaxValue);
                factory.Export.RecordExport += OnRecordImportExport;
                factory.Export.Export(sightings, arguments[0]);
                Console.WriteLine($"\nExported the database to {arguments[0]}");
            }
        }
    }
}