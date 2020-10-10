using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ReportCommand : ReportCommandBase
    {
        public ReportCommand()
        {
            Type = CommandType.report;
            MinimumArguments = 1;
            MaximiumArguments = 5;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument is the report type, that determines the
                // required argument count
                ReportType type = GetReportType(context);
                if (ArgumentCountCorrectForReportType(context, type))
                {
                    switch (type)
                    {
                        case ReportType.Summary:
                            ShowSummaryReport(context);
                            break;
                        case ReportType.Location:
                            ShowLocationReport(context);
                            break;
                        case ReportType.Category:
                            ShowCategoryReport(context);
                            break;
                        case ReportType.Species:
                            ShowSpeciesReport(context);
                            break;
                        case ReportType.Status:
                            ShowStatusReport(context);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Show the species report for a specified date range and location
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ShowSpeciesReport(CommandContext context)
        {
            // Extract the report filters from the arguments
            int? speciesId = GetSpecies(context, 1);
            int? locationId = GetLocation(context, 2);
            DateTime? defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime? from = GetDateFromArgument(context.Arguments, 3, defaultDate, context.Output);
            DateTime? to = GetDateFromArgument(context.Arguments, 4, defaultDate, context.Output);
            if ((from != null) && (to != null))
            {
                DateTime reportFrom = from ?? DateTime.Now;
                DateTime reportTo = to ?? DateTime.Now;
                Summarise(context.Factory, reportFrom, reportTo, locationId, null, speciesId, context.Output);
            }
            context.Output.Flush();
        }

        /// <summary>
        /// Show the category report for a specified date range and location
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ShowCategoryReport(CommandContext context)
        {
            // Extract the report filters from the arguments
            int? categoryId = GetCategory(context, 1);
            int? locationId = GetLocation(context, 2);
            DateTime? defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime? from = GetDateFromArgument(context.Arguments, 3, defaultDate, context.Output);
            DateTime? to = GetDateFromArgument(context.Arguments, 4, defaultDate, context.Output);
            if ((from != null) && (to != null))
            {
                DateTime reportFrom = from ?? DateTime.Now;
                DateTime reportTo = to ?? DateTime.Now;
                Summarise(context.Factory, reportFrom, reportTo, locationId, categoryId, null, context.Output);
            }
            context.Output.Flush();
        }

        /// <summary>
        /// Show the location report for a specified date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ShowLocationReport(CommandContext context)
        {
            // Extract the report filters from the arguments
            int? locationId = GetLocation(context, 1);
            DateTime? defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime? from = GetDateFromArgument(context.Arguments, 2, defaultDate, context.Output);
            DateTime? to = GetDateFromArgument(context.Arguments, 3, defaultDate, context.Output);
            if ((from != null) && (to != null))
            {
                DateTime reportFrom = from ?? DateTime.Now;
                DateTime reportTo = to ?? DateTime.Now;
                Summarise(context.Factory, reportFrom, reportTo, locationId, null, null, context.Output);
            }
            context.Output.Flush();
        }

        /// <summary>
        /// Show the summary report for a specified date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ShowSummaryReport(CommandContext context)
        {
            // Extract the report filters from the arguments
            DateTime? defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime? from = GetDateFromArgument(context.Arguments, 1, defaultDate, context.Output);
            DateTime? to = GetDateFromArgument(context.Arguments, 2, defaultDate, context.Output);
            if ((from != null) && (to != null))
            {
                DateTime reportFrom = from ?? DateTime.Now;
                DateTime reportTo = to ?? DateTime.Now;
                Summarise(context.Factory, reportFrom, reportTo, null, null, null, context.Output);
            }
            context.Output.Flush();
        }

        /// <summary>
        /// Show the conservation status summary report for a specified species and scheme
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ShowStatusReport(CommandContext context)
        {
            string speciesName = context.CleanArgument(1);
            string schemeName = (context.Arguments.Length == 3) ? context.CleanArgument(2) : null;
            SummariseConservationStatus(context.Factory, speciesName, schemeName, context.Output);
        }

        /// <summary>
        /// Check the argument count is correct for the specified report type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool ArgumentCountCorrectForReportType(CommandContext context, ReportType type)
        {
            (int minimum, int maximum) counts = GetRequiredArgumentCounts(type);
            bool correct = ((context.Arguments.Length >= counts.minimum) && (context.Arguments.Length <= counts.maximum));
            if (!correct)
            {
                context.Output.WriteLine($"Incorrect argument count for the \"{Type} {context.Arguments[0]}\" command");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Return the argument limits based on the report type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private (int minimum, int maximum) GetRequiredArgumentCounts(ReportType type)
        {
            (int minimum, int maximum) counts;

            switch (type)
            {
                case ReportType.Summary:
                    counts.minimum = 1;
                    counts.maximum = 3;
                    break;
                case ReportType.Location:
                    counts.minimum = 2;
                    counts.maximum = 4;
                    break;
                case ReportType.Category:
                    counts.minimum = 3;
                    counts.maximum = 5;
                    break;
                case ReportType.Species:
                    counts.minimum = 3;
                    counts.maximum = 5;
                    break;
                case ReportType.Status:
                    counts.minimum = 2;
                    counts.maximum = 3;
                    break;
                default:
                    string message = "Cannot generate unknown report type";
                    throw new UnknownReportTypeException(message);
            }

            return counts;
        }

        /// <summary>
        /// Determine the required report type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private ReportType GetReportType(CommandContext context)
        {
            string reportTypeName = context.CleanArgument(0);
            if (!Enum.TryParse<ReportType>(reportTypeName, out ReportType type))
            {
                string message = "Cannot generate unknown report type";
                throw new UnknownReportTypeException(message);
            }

            return type;
        }
    }
}
