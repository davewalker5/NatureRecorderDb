using System;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class DeleteUserCommand : CommandBase
    {
        public DeleteUserCommand()
        {
            Type = CommandType.deleteuser;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                factory.Users.DeleteUser(arguments[0]);
                Console.WriteLine($"Deleted user {arguments[0]}");
            }
        }
    }
}
