using System;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class RenameCommand : CommandBase
    {
        public RenameCommand()
        {
            Type = CommandType.rename;
            MinimumArguments = 3;
            MaximiumArguments = 3;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being renamed
                string entityTypeName = context.CleanArgument(0);
                if (Enum.TryParse<EntityType>(entityTypeName, out EntityType entityType))
                {
                    switch (entityType)
                    {
                        case EntityType.Category:
                            context.Factory
                                   .Categories
                                   .Rename(context.Arguments[1], context.Arguments[2]);
                            break;
                        case EntityType.Species:
                            context.Factory
                                   .Species
                                   .Rename(context.Arguments[1], context.Arguments[2]);
                            break;
                        default:
                            string message = $"Cannot rename unknown entity type";
                            throw new UnknownEntityTypeException(message);
                    }

                    // Report the rename
                    context.Output.WriteLine($"{context.Arguments[0]} '{context.Arguments[1]}' renamed to '{context.Arguments[2]}'");
                    context.Output.Flush();
                }
                else
                {
                    string message = $"Cannot rename unknown entity type";
                    throw new UnknownEntityTypeException(message);
                }
            }
        }
    }
}
