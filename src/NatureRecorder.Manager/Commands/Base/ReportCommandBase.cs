using System;
using System.Globalization;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Reporting;

namespace NatureRecorder.Manager.Commands.Base
{
    public abstract class ReportCommandBase : CommandBase
    {
        private const string DateFormat = "yyyy-MM-dd";

        public void Summarise(NatureRecorderFactory factory, DateTime from, DateTime to)
        {
            // Retrieve the summary for the specified date
            Summary summary = factory.Sightings.Summarise(from, to);
            if (summary.Sightings.Any())
            {
                Console.WriteLine($"Summary of sightings from {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}:\n");

                string sightingsSuffix = (summary.Sightings.Count() > 1) ? "s" : "";
                Console.WriteLine($"\t{summary.Sightings.Count()} sighting{sightingsSuffix}");
                Console.WriteLine($"\t{summary.Species.Count()} species");

                string categoriesSuffix = (summary.Categories.Count() > 1) ? "ies" : "y";
                Console.WriteLine($"\t{summary.Categories.Count()} categor{categoriesSuffix}");

                string locationsSuffix = (summary.Locations.Count() > 1) ? "s" : "";
                Console.WriteLine($"\t{summary.Locations.Count()} location{locationsSuffix}\n");

                SightingsTable table = new SightingsTable(summary.Sightings);
                table.PrintTable();
            }
            else
            {
                Console.WriteLine($"There were no sightings on {from.ToShortDateString()}");
            }
        }

        /// <summary>
        /// Return the date/time represented by the input string or NULL if the format
        /// isn't as expected
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected DateTime? GetDateFromArgument(string argument)
        {
            DateTime? result = null;
            DateTime parsed;

            if (DateTime.TryParseExact(argument, DateFormat, null, DateTimeStyles.None, out parsed))
            {
                result = parsed;
            }
            else
            {
                Console.WriteLine($"\"{argument}\" is not in the expected format ({DateFormat})");
            }

            return result;
        }
    }
}
