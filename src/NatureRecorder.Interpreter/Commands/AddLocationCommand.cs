using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddLocationCommand : AddCommandBase
    {
        public AddLocationCommand()
        {
            Type = CommandType.addlocation;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                Location template = ReadDetails(context);
                if (template != null)
                {
                    Location location = context.Factory.Locations.Add(template.Name,
                                                                      template.Address,
                                                                      template.City,
                                                                      template.County,
                                                                      template.Postcode,
                                                                      template.Country,
                                                                      template.Latitude,
                                                                      template.Longitude);
                    context.Output.WriteLine($"Location {location.Name} added");
                    context.Output.Flush();
                }
            }
        }

        /// <summary>
        /// Prompt for the new location details
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Location ReadDetails(CommandContext context)
        {
            // Prompt for the location details
            Location location = PromptForLocation(context, null, false);
            if (context.Reader.Cancelled) return null;

            location.Address = context.Reader.ReadLine("Address", true);
            if (context.Reader.Cancelled) return null;

            location.City = context.Reader.ReadLine("City", true);
            if (context.Reader.Cancelled) return null;

            location.County = context.Reader.ReadLine("County", true);
            if (context.Reader.Cancelled) return null;

            location.Postcode = context.Reader.ReadLine("Postcode", true);
            if (context.Reader.Cancelled) return null;

            location.Country = context.Reader.ReadLine("Country", true);
            if (context.Reader.Cancelled) return null;

            location.Latitude = context.Reader.Read<decimal?>("Latitude");
            if (context.Reader.Cancelled) return null;

            location.Longitude = context.Reader.Read<decimal?>("Longitude");
            if (context.Reader.Cancelled) return null;

            return location;
        }
    }
}
