using System;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ReportCommand : ReportCommandBase
    {
        public ReportCommand()
        {
            Type = CommandType.report;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                // The date-time parser returns NULL (and displays an error message) if it
                // can't get a date from the specified string
                DateTime? from = GetDateFromArgument(context.Arguments[0], context.Output);
                DateTime? to = GetDateFromArgument(context.Arguments[0], context.Output);
                if ((from != null) && (to != null))
                {
                    DateTime reportFrom = from ?? DateTime.Now;
                    DateTime reportTo = to ?? DateTime.Now;
                    Summarise(context.Factory, reportFrom, reportTo, context.Output);
                }
            }
        }
    }
}
