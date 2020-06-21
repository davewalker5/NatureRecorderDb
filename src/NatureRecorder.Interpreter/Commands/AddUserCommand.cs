using System;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddUserCommand : CommandBase
    {
        public AddUserCommand()
        {
            Type = CommandType.adduser;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Users.AddUser(context.Arguments[0], context.Arguments[1]);
                context.Output.WriteLine($"Added user {context.Arguments[0]}");
                context.Output.Flush();
            }
        }
    }
}
