using System;
using System.IO;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Reporting;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class ReportCommandBase : CommandBase
    {
        /// <summary>
        /// Create a summary report for the specified report filters
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="locationId"></param>
        /// <param name="categoryId"></param>
        /// <param name="speciesId"></param>
        /// <param name="output"></param>
        public void Summarise(NatureRecorderFactory factory, DateTime from, DateTime to, int? locationId, int? categoryId, int? speciesId, StreamWriter output)
        {
            Summary summary = factory.Sightings.Summarise(from, to, locationId, categoryId, speciesId);
            if (summary.Sightings.Any())
            {
                output.WriteLine($"Summary of sightings from {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}:\n");

                string sightingsSuffix = (summary.Sightings.Count() > 1) ? "s" : "";
                output.WriteLine($"\t{summary.Sightings.Count()} sighting{sightingsSuffix}");
                output.WriteLine($"\t{summary.Species.Count()} species");

                string categoriesSuffix = (summary.Categories.Count() > 1) ? "ies" : "y";
                output.WriteLine($"\t{summary.Categories.Count()} categor{categoriesSuffix}");

                string locationsSuffix = (summary.Locations.Count() > 1) ? "s" : "";
                output.WriteLine($"\t{summary.Locations.Count()} location{locationsSuffix}\n");

                SightingsTable table = new SightingsTable(summary.Sightings);
                table.PrintTable(output);
            }
            else
            {
                output.WriteLine($"There were no sightings on {from.ToShortDateString()}");
                output.Flush();
            }
        }
    }
}
