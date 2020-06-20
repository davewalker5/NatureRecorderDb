using System.Collections.Generic;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Reporting;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ListLocationsCommand : CommandBase
    {
        public ListLocationsCommand()
        {
            Type = CommandType.locations;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                IEnumerable<Location> locations = context.Factory.Locations.List(null, 1, int.MaxValue);
                if (locations.Any())
                {
                    context.Output.WriteLine($"There are {locations.Count()} locations in the database:\n");
                    LocationsTable table = new LocationsTable(locations);
                    table.PrintTable(context.Output);
                }
                else
                {
                    context.Output.WriteLine("There are no locations in the database");
                    context.Output.Flush();
                }
            }
        }
    }
}
