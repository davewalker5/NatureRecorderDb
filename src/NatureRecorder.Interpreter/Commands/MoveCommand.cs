using System;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class MoveCommand : CommandBase
    {
        public MoveCommand()
        {
            Type = CommandType.move;
            MinimumArguments = 3;
            MaximiumArguments = 3;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument indicates the kind of entity being moved
                string entityTypeName = context.CleanArgument(0);
                if (Enum.TryParse<EntityType>(entityTypeName, out EntityType entityType))
                {
                    switch (entityType)
                    {
                        case EntityType.Species:
                            context.Factory
                                   .Species
                                   .Move(context.Arguments[1], context.Arguments[2]);
                            break;
                        default:
                            string message = $"Cannot move unknown entity type";
                            throw new UnknownEntityTypeException(message);
                    }

                    // Report the rename
                    context.Output.WriteLine($"{context.Arguments[0]} '{context.Arguments[1]}' moved to '{context.Arguments[2]}'");
                    context.Output.Flush();
                }
                else
                {
                    string message = $"Cannot move unknown entity type";
                    throw new UnknownEntityTypeException(message);
                }
            }
        }
    }
}
