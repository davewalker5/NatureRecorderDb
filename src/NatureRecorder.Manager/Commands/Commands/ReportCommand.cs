using System;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ReportCommand : ReportCommandBase
    {
        public ReportCommand()
        {
            Type = CommandType.report;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // The date-time parser returns NULL (and displays an error message) if it
                // can't get a date from the specified string
                DateTime? from = GetDateFromArgument(arguments[0]);
                DateTime? to = GetDateFromArgument(arguments[0]);
                if ((from != null) && (to != null))
                {
                    DateTime reportFrom = from ?? DateTime.Now;
                    DateTime reportTo = to ?? DateTime.Now;
                    Summarise(factory, reportFrom, reportTo);
                }
            }
        }
    }
}
