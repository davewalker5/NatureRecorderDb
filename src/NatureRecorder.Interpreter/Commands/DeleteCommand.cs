using System;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class DeleteCommand : CommandBase
    {
        public DeleteCommand()
        {
            Type = CommandType.delete;
            MinimumArguments = 2;
            MaximiumArguments = 3;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being deleted
                string entityTypeName = context.CleanArgument(0);
                if (Enum.TryParse<EntityType>(entityTypeName, out EntityType entityType))
                {
                    if (ArgumentCountCorrectForEntityType(context, entityType))
                    {
                        switch (entityType)
                        {
                            case EntityType.Location:
                                context.Factory.Locations
                                                .Delete(context.Arguments[1]);
                                break;
                            case EntityType.Category:
                                context.Factory.Categories
                                                .Delete(context.Arguments[1]);
                                break;
                            case EntityType.Species:
                                context.Factory.Species
                                                .Delete(context.Arguments[1]);
                                break;
                            case EntityType.Sighting:
                                if (int.TryParse(context.Arguments[1], out int id))
                                {
                                    context.Factory.Sightings
                                                    .Delete(id);
                                }
                                else
                                {
                                    string message = $"'{context.Arguments[1]}' is not a valid sighting ID";
                                    throw new InvalidIdentifierException(message);
                                }
                                break;
                            case EntityType.User:
                                context.Factory
                                        .Users
                                        .DeleteUser(context.Arguments[1]);
                                break;
                            case EntityType.Scheme:
                                context.Factory.StatusSchemes.Delete(context.Arguments[1]);
                                break;
                            case EntityType.Rating:
                                context.Factory.StatusRatings.Delete(context.Arguments[1], context.Arguments[2]);
                                break;
                        }

                        // Report the rename
                        context.Output.WriteLine($"Deleted {context.Arguments[0]} '{context.Arguments[1]}'");
                        context.Output.Flush();
                    }
                }
                else
                {
                    string message = $"Cannot delete unknown entity type";
                    throw new UnknownEntityTypeException(message);
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
                case EntityType.Rating:
                    counts.minimum = 3;
                    counts.maximum = 3;
                    break;
                default:
                    counts.minimum = 2;
                    counts.maximum = 2;
                    break;
            }

            return counts;
        }
    }
}
