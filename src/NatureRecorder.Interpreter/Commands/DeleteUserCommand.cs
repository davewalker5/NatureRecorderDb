using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class DeleteUserCommand : CommandBase
    {
        public DeleteUserCommand()
        {
            Type = CommandType.deleteuser;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                context.Factory.Users.DeleteUser(context.Arguments[0]);
                context.Output.WriteLine($"Deleted user {context.Arguments[0]}");
                context.Output.Flush();
            }
        }
    }
}
