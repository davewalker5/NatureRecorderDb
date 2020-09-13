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
            MinimumArguments = 0;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // If the date isn't specified, use today's date
                DateTime? date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (context.Arguments.Length > 0)
                {
                    // The date-time parser returns NULL (and displays an error message) if it
                    // can't get a date from the specified string
                    date = GetDateFromArgument(context.Arguments[0], context.Output);
                }

                if (date != null)
                {
                    DateTime reportDate = date ?? DateTime.Now;
                    Summarise(context.Factory, reportDate, reportDate, context.Output);
                }
            }
        }
    }
}
