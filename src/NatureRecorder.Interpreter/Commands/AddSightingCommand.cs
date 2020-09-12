using System;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddSightingCommand : AddCommandBase
    {
        public AddSightingCommand()
        {
            Type = CommandType.addsighting;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                bool more;

                do
                {
                    // Output a spacer line
                    context.Output.WriteLine();
                    context.Output.Flush();

                    // Prompt for the location
                    string defaultValue = (context.CurrentLocation != null) ? context.CurrentLocation.Name : null;
                    Location location = PromptForLocation(context, defaultValue, true);
                    if (location == null) return;

                    // Prompt for the date of sighting
                    defaultValue = (context.CurrentDate ?? DateTime.Now).ToString(DateFormat);
                    DateTime? date = PromptForDate(context, defaultValue);
                    if (date == null) return;

                    // Prompt for the species
                    Species species = PromptForSpecies(context, null, true);
                    if (species == null) return;

                    // Prompt for the number of individuals seen
                    int? number = PromptForNumber(context, "0");
                    if (number == null) return;

                    // Yes/No prompt for whether or not young were also seen
                    bool withYoung = context.Reader.PromptForYesNo("Seen with young");

                    // Update the current date and location
                    context.CurrentDate = date;
                    context.CurrentLocation = location;

                    // See if there's an existing record for that species, date and location
                    DateTime timestamp = date ?? DateTime.Now;
                    Sighting sighting = context.Factory
                                               .Sightings
                                               .List(s => (s.Date == timestamp) && (s.SpeciesId == species.Id) && (s.LocationId == location.Id), 1, int.MaxValue)
                                               .FirstOrDefault();

                    // If a sighting exists, update it. Otherwise, add a new one
                    string action;
                    if (sighting != null)
                    {
                        action = "Updated";
                        sighting.Number = number ?? 0;
                        sighting.WithYoung = withYoung;
                        context.Factory.Context.SaveChanges();
                    }
                    else
                    {
                        action = "Added";
                        context.Factory
                               .Sightings
                               .Add(number ?? 0, withYoung, timestamp, location.Id, species.Id);
                    }

                    string numberReportString = (number > 0) ? $"{number} x " : "";
                    context.Output.WriteLine($"{action} sighting of {numberReportString}{species.Name} at {location.Name} on {timestamp.ToString(DateFormat)}");
                    context.Output.Flush();

                    // Prompt for addition of another sighting
                    more = context.Reader.PromptForYesNo("\nAdd another?");
                }
                while (more);
            }
        }
    }
}
