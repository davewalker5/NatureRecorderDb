using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddCommand : AddEditCommandBase
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
                if (Enum.TryParse<EntityType>(entityTypeName, out EntityType entityType))
                {
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
                        }
                    }
                }
                else
                {
                    string message = $"Cannot add unknown entity type";
                    throw new UnknownEntityTypeException(message);
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
            Location template = PromptForLocation(context, null);
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
                Species template = PromptForSpecies(context, category.Id, false, null, false);
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
                // Get the sighting details
                Sighting template = PromptForSighting(context, null);
                if (template == null) return;

                // Update the current date and location
                context.CurrentDate = template.Date;
                context.CurrentLocation = template.Location;

                // See if there's an existing record for that species, date and location
                Sighting sighting = context.Factory
                                           .Sightings
                                           .List(s => (s.Date == template.Date) && (s.SpeciesId == template.Species.Id) && (s.LocationId == template.LocationId), 1, int.MaxValue)
                                           .FirstOrDefault();

                // If a sighting exists, update it. Otherwise, add a new one
                string action;
                if (sighting != null)
                {
                    action = "Updated";
                    sighting.Number = template.Number;
                    sighting.WithYoung = template.WithYoung;
                    context.Factory.Context.SaveChanges();
                }
                else
                {
                    action = "Added";
                    context.Factory
                           .Sightings
                           .Add(template);
                }

                string numberReportString = (template.Number > 0) ? $"{template.Number} x " : "";
                context.Output.WriteLine($"{action} sighting of {numberReportString}{template.Species.Name} at {template.Location.Name} on {template.Date.ToString(DateFormat)}");
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
