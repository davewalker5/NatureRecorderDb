using System;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class SummaryCommand : ReportCommandBase
    {
        public SummaryCommand()
        {
            Type = CommandType.summary;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                // The date-time parser returns NULL (and displays an error message) if it
                // can't get a date from the specified string
                DateTime? date = GetDateFromArgument(context.Arguments[0], context.Output);
                if (date != null)
                {
                    DateTime reportDate = date ?? DateTime.Now;
                    Summarise(context.Factory, reportDate, reportDate, context.Output);
                }
            }
        }
    }
}
