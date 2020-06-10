using System;
using System.Collections.Generic;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Reporting;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ListLocationsCommand : CommandBase
    {
        public ListLocationsCommand()
        {
            Type = CommandType.locations;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                IEnumerable<Location> locations = factory.Locations.List(null, 1, int.MaxValue);
                if (locations.Any())
                {
                    Console.WriteLine($"There are {locations.Count()} locations in the database:\n");
                    LocationsTable table = new LocationsTable(locations);
                    table.PrintTable();
                }
                else
                {
                    Console.WriteLine("There are no locations in the database");
                }
            }
        }
    }
}
