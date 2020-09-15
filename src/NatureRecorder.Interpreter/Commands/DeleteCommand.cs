using System;
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
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being deleted
                string entityTypeName = context.CleanArgument(0);
                EntityType entityType = Enum.Parse<EntityType>(entityTypeName);

                string message;
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
                            message = $"'{context.Arguments[1]}' is not a valid sighting ID";
                            throw new InvalidIdentifierException(message);
                        }
                        break;
                    case EntityType.User:
                        context.Factory
                                .Users
                                .DeleteUser(context.Arguments[1]);
                        break;
                    default:
                        message = $"Cannot delete unknown entity type";
                        throw new UnknownEntityType(message);
                }

                // Report the rename
                context.Output.WriteLine($"Deleted {context.Arguments[0]} '{context.Arguments[1]}'");
                context.Output.Flush();
            }
        }
    }
}
