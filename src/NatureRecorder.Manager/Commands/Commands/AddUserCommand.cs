using System;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class AddUserCommand : CommandBase
    {
        public AddUserCommand()
        {
            Type = CommandType.adduser;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                factory.Users.AddUser(arguments[0], arguments[1]);
                Console.WriteLine($"Added user {arguments[0]}");
            }
        }
    }
}
