using System;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class HistoryCommand : CommandBase
    {
        public HistoryCommand()
        {
            Type = CommandType.history;
            MinimumArguments = 0;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context) && (context.History != null))
            {
                (HistoryAction action, int entry) action = GetAction(context);
                switch (action.action)
                {
                    case HistoryAction.List:
                        context.History.List(context.Output);
                        break;
                    case HistoryAction.Clear:
                        ClearHistory(context);
                        break;
                    case HistoryAction.Location:
                        context.History.Location(context.Output);
                        break;
                    case HistoryAction.Recall:
                        context.RecalledCommand = context.History.Get(action.entry);
                        break;
                    default:
                        string message = $"Unrecognised history action";
                        throw new UnrecognisedHistoryActionException(message);
                }
            }
        }

        /// <summary>
        /// Clear the command history
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ClearHistory(CommandContext context)
        {
            context.History.Clear();
            context.Output.WriteLine("Command history has been cleared");
            context.Output.Flush();
        }

        /// <summary>
        /// Determine the specific action to take
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private (HistoryAction action, int entry) GetAction(CommandContext context)
        {
            HistoryAction action = HistoryAction.Unknown;
            int entry = 0;

            if (context.Arguments.Length == 0)
            {
                // No arguments, just list the current history
                action = HistoryAction.List;
            }
            else if (int.TryParse(context.Arguments[0], out entry))
            {
                // Recall and execute the specified command entry
                action = HistoryAction.Recall;
            }
            else
            {
                // One of the predefined history actions
                string actionText = context.CleanArgument(0);
                if (!Enum.TryParse<HistoryAction>(actionText, out action))
                {
                    action = HistoryAction.Unknown;
                }
            }

            return (action, entry);
        }
    }
}
