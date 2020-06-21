using System;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class SetPasswordCommand : CommandBase
    {
        public SetPasswordCommand()
        {
            Type = CommandType.setpassword;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Users.SetPassword(context.Arguments[0], context.Arguments[1]);
                context.Output.WriteLine($"Set password for user {context.Arguments[0]}");
                context.Output.Flush();
            }
        }
    }
}
