using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddCommand : AddCommandBase
    {
        public AddCommand()
        {
            Type = CommandType.add;
            MinimumArguments = 1;
            MaximiumArguments = 3;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            bool validForCommandMode = ValidForCommandMode(context);
            if (validForCommandMode && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being added
                string entityTypeName = context.CleanArgument(0);
                EntityType entityType = Enum.Parse<EntityType>(entityTypeName);

                // The "add" command has to be valid for all modes to accommodate
                // all the variations. However, only "add user" is valid in a context
                // other than "interactive", so revalidate now we know the entity
                // type being added
                if (entityType != EntityType.User)
                {
                    RequiredMode = CommandMode.Interactive;
                    validForCommandMode = ValidForCommandMode(context);
                }

                // The entity type also determines the required argument count
                if (validForCommandMode && ArgumentCountCorrectForEntityType(context, entityType))
                {
                    string message;
                    switch (entityType)
                    {
                        case EntityType.Location:
                            AddLocation(context);
                            break;
                        case EntityType.Category:
                            AddCategory(context);
                            break;
                        case EntityType.Species:
                            AddSpecies(context);
                            break;
                        case EntityType.Sighting:
                            AddSighting(context);
                            break;
                        case EntityType.User:
                            AddUser(context);
                            break;
                        default:
                            message = $"Cannot add unknown entity type";
                            throw new UnknownEntityType(message);
                    }
                }
            }
        }

        /// <summary>
        /// Add a location
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void AddLocation(CommandContext context)
        {
            Location template = ReadLocationDetails(context);
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

        /// <summary>
        /// Prompt for the new location details
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private Location ReadLocationDetails(CommandContext context)
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

        /// <summary>
        /// Add a category
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void AddCategory(CommandContext context)
        {
            Category template = PromptForCategory(context, false);
            if (template != null)
            {
                Category category = context.Factory.Categories.Add(template.Name);
                context.Output.WriteLine($"Category {category.Name} added");
                context.Output.Flush();
            }
        }

        /// <summary>
        /// Add a species
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void AddSpecies(CommandContext context)
        {
            Category category = PromptForCategory(context, true);
            if (category != null)
            {
                Species template = PromptForSpecies(context, category.Id, false);
                if (template != null)
                {
                    Species species = context.Factory.Species.Add(template.Name, category.Name);
                    context.Output.WriteLine($"Species {species.Name} added to {category.Name}");
                    context.Output.Flush();
                }
            }
        }

        /// <summary>
        /// Add a sighting
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void AddSighting(CommandContext context)
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

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void AddUser(CommandContext context)
        {
            context.Factory.Users.AddUser(context.Arguments[1], context.Arguments[2]);
            context.Output.WriteLine($"Added user {context.Arguments[1]}");
            context.Output.Flush();
        }

        /// <summary>
        /// Check the argument count is correct for the specified entity type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool ArgumentCountCorrectForEntityType(CommandContext context, EntityType type)
        {
            (int minimum, int maximum) counts = GetRequiredArgumentCounts(type);
            bool correct = ((context.Arguments.Length >= counts.minimum) && (context.Arguments.Length <= counts.maximum));
            if (!correct)
            {
                context.Output.WriteLine($"Incorrect argument count for the \"{Type} {context.Arguments[0]}\" command");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Return the argument limits based on the entity type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private (int minimum, int maximum) GetRequiredArgumentCounts(EntityType type)
        {
            (int minimum, int maximum) counts;

            switch (type)
            {
                case EntityType.User:
                    counts.minimum = 3;
                    counts.maximum = 3;
                    break;
                default:
                    counts.minimum = 1;
                    counts.maximum = 1;
                    break;
            }

            return counts;
        }
    }
}
