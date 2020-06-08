using System;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class SetPasswordCommand : CommandBase
    {
        public SetPasswordCommand()
        {
            Type = CommandType.setpassword;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                factory.Users.SetPassword(arguments[0], arguments[1]);
                Console.WriteLine($"Set password for user {arguments[0]}");
            }
        }
    }
}
