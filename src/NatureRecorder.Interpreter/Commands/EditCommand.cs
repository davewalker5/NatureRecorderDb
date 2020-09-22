using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class EditCommand : AddEditCommandBase
    {
        public EditCommand()
        {
            Type = CommandType.edit;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being added
                string entityTypeName = context.CleanArgument(0);
                if (Enum.TryParse<EntityType>(entityTypeName, out EntityType entityType))
                {
                    switch (entityType)
                    {
                        case EntityType.Location:
                            EditLocation(context);
                            break;
                        case EntityType.Sighting:
                            EditSighting(context);
                            break;
                        default:
                            string message = $"Cannot edit unknown entity type";
                            throw new UnknownEntityTypeException(message);
                    }
                }
                else
                {
                    string message = $"Cannot edit unknown entity type";
                    throw new UnknownEntityTypeException(message);
                }
            }
        }

        /// <summary>
        /// Edit an existing sighting
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void EditSighting(CommandContext context)
        {
            // The arguments specify the sighting ID, that must correspond to
            // an existing location
            Sighting sighting = null;
            if (int.TryParse(context.Arguments[1], out int id))
            {
                sighting = context.Factory.Sightings
                                          .Get(s => s.Id == id);
            }

            if (sighting == null)
            {
                string message = $"Sighting '{context.Arguments[1]}' does not exist";
                throw new SightingDoesNotExistException(message);
            }

            // Prompt for sighting details, using the current values as defaults
            Sighting updated = PromptForSighting(context, sighting);
            if (updated != null)
            {
                // Save the changes to the database
                context.Factory.Context.Entry(sighting).State = EntityState.Detached;
                context.Factory.Context.Entry(updated).State = EntityState.Modified;
                context.Factory.Context.SaveChanges();

                // Report the update
                context.Output.WriteLine($"Updated sighting '{sighting.Id}");
                context.Output.Flush();
            }
        }

        /// <summary>
        /// Edit an existing location
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void EditLocation(CommandContext context)
        {
            // The arguments specify the location name, that must correspond to
            // an existing location
            string name = context.CleanArgument(1);
            Location location = context.Factory.Locations
                                               .Get(l => l.Name == name);
            if (location == null)
            {
                string message = $"Location '{name}' does not exist";
                throw new LocationDoesNotExistException(message);
            }

            // Prompt for location details, using the current values as defaults
            Location updated = PromptForLocation(context, location);
            if (updated != null)
            {
                // Save the changes to the database
                context.Factory.Context.Entry(location).State = EntityState.Detached;
                context.Factory.Context.Entry(updated).State = EntityState.Modified;
                context.Factory.Context.SaveChanges();

                // Report the update
                context.Output.WriteLine($"Updated location '{location.Name}");
                context.Output.Flush();
            }
        }
    }
}
