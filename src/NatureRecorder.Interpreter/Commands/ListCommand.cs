using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Reporting;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ListCommand : CommandBase
    {
        public ListCommand()
        {
            Type = CommandType.list;
            MinimumArguments = 1;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being moved
                // that, in turn, determines how many arguments we actually need
                EntityType entityType = GetEntityType(context);
                if (ArgumentCountCorrectForEntityType(context, entityType))
                {
                    switch (entityType)
                    {
                        case EntityType.Location:
                            ListLocations(context);
                            break;
                        case EntityType.Category:
                            ListCategories(context);
                            break;
                        case EntityType.Species:
                            ListSpecies(context);
                            break;
                        default:
                            string message = $"Cannot list unknown entity type";
                            throw new UnknownEntityType(message);
                    }
                }
            }
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
            int requiredCount = GetRequiredArgumentCount(type);
            bool correct = (context.Arguments.Length == requiredCount);
            if (!correct)
            {
                context.Output.WriteLine($"Incorrect argument count for the \"{Type} {context.Arguments[0]}\" command");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Return the required argument count based on the entity type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private int GetRequiredArgumentCount(EntityType type)
        {
            int count;

            switch (type)
            {
                case EntityType.Location:
                    count = 1;
                    break;
                case EntityType.Category:
                    count = 1;
                    break;
                case EntityType.Species:
                    count = 2;
                    break;
                default:
                    string message = $"Cannot list unknown entity type";
                    throw new UnknownEntityType(message);
            }

            return count;
        }

        /// <summary>
        /// Determine the entity type to list
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private EntityType GetEntityType(CommandContext context)
        {
            EntityType type;

            // Note we don't use Enum.TryParse<T>() to determine the entity
            // types from the argument. The entity type enumeration contains
            // the singular for each entity type whereas the command expects
            // the plural
            //
            // e.g. list categories
            string entityTypeName = context.CleanArgument(0);
            switch (entityTypeName)
            {
                case "Locations":
                    type = EntityType.Location;
                    break;
                case "Categories":
                    type = EntityType.Category;
                    break;
                case "Species":
                    type = EntityType.Species;
                    break;
                default:
                    string message = $"Cannot list unknown entity type";
                    throw new UnknownEntityType(message);
            }

            return type;
        }

        /// <summary>
        /// List the locations in the DB
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ListLocations(CommandContext context)
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
            }

            context.Output.Flush();
        }

        /// <summary>
        /// List the categories in the DB
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ListCategories(CommandContext context)
        {
            IEnumerable<Category> categories = context.Factory.Categories.List(null, 1, int.MaxValue);
            if (categories.Any())
            {
                context.Output.WriteLine($"There are {categories.Count()} species categories in the database:\n");
                foreach (Category category in categories.OrderBy(l => l.Name))
                {
                    context.Output.WriteLine($"\t{category.Name}");
                }
            }
            else
            {
                context.Output.WriteLine("There are no species categories in the database");
            }

            context.Output.Flush();
        }

        /// <summary>
        /// List the species for the category supplied in the arguments
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ListSpecies(CommandContext context)
        {
            // Get the category details from the database
            string categoryName = ToTitleCase(context.Arguments[1]);
            Category category = context.Factory.Categories.Get(c => c.Name == categoryName);
            if (category != null)
            {
                // Get a list of species in that category
                IEnumerable<Species> matches = context.Factory
                                                      .Species
                                                      .List(s => s.CategoryId == category.Id, 1, int.MaxValue);
                if (matches.Any())
                {
                    // Show the results
                    context.Output.WriteLine($"There are {matches.Count()} species listed for category \"{categoryName}\":\n");
                    foreach (Species species in matches.OrderBy(l => l.Name))
                    {
                        context.Output.WriteLine($"\t{species.Name}");
                    }
                }
                else
                {
                    context.Output.WriteLine($"There are no species for category \"{categoryName}\" in the database");
                }
            }
            else
            {
                context.Output.WriteLine($"Species category \"{categoryName}\" does not exist");
            }

            context.Output.Flush();
        }
    }
}
