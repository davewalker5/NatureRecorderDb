using System;
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

                    // Update the current date and location
                    context.CurrentDate = date;
                    context.CurrentLocation = location;

                    // Add the sighting
                    DateTime timestamp = date ?? DateTime.Now;
                    Sighting sighting = context.Factory
                                               .Sightings
                                               .Add(0, timestamp, location.Id, species.Id);
                    context.Output.WriteLine($"Added sighting of {species.Name} at {location.Name} on {timestamp.ToString(DateFormat)}");
                    context.Output.Flush();

                    // Prompt for addition of another sighting
                    more = context.Reader.PromptForYesNo("\nAdd another?");
                }
                while (more);
            }
        }
    }
}
