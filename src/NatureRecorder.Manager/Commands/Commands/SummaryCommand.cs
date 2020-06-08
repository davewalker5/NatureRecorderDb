using System;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class SummaryCommand : ReportCommandBase
    {
        public SummaryCommand()
        {
            Type = CommandType.summary;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // The date-time parser returns NULL (and displays an error message) if it
                // can't get a date from the specified string
                DateTime? date = GetDateFromArgument(arguments[0]);
                if (date != null)
                {
                    DateTime reportDate = date ?? DateTime.Now;
                    Summarise(factory, reportDate, reportDate);
                }
            }
        }
    }
}
