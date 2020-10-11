using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
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

        /// <summary>
        /// Summarise the conservation ratings for a specified species and scheme
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="speciesName"></param>
        /// <param name="schemeName"></param>
        /// <param name="atDate"></param>
        /// <param name="output"></param>
        public void SummariseConservationStatus(NatureRecorderFactory factory, string speciesName, string schemeName, DateTime? atDate, StreamWriter output)
        {
            Expression<Func<SpeciesStatusRating, bool>> predicate;
            string title;
            string noMatchesMessage;

            // Construct the messages and filtering predicate based on the arguments
            if (schemeName == null)
            {
                // Summary of conservation status for species against all schemes
                predicate = r => (r.Species.Name == speciesName);
                title = $"Conservation status summary for {speciesName}";
                noMatchesMessage = $"There are no ratings for species {speciesName}";
            }
            else if (atDate == null)
            {
                // Summary of conservation status for species against a specific scheme
                predicate = r => (r.Species.Name == speciesName) && (r.Rating.Scheme.Name == schemeName);
                title = $"Conservation status summary for {speciesName} using scheme {schemeName}";
                noMatchesMessage = $"There are no ratings for species {speciesName} using scheme {schemeName}";
            }
            else
            {
                // Summary of conservation status for species against a specific scheme at a given date
                predicate = r => (r.Species.Name == speciesName) &&
                                 (r.Rating.Scheme.Name == schemeName) &&
                                 ((r.Start == null) || (r.Start <= atDate)) &&
                                 ((r.End == null) || (r.End >= atDate));
                title = $"Conservation status summary for {speciesName} using scheme {schemeName} at {(atDate ?? DateTime.Now).ToString("dd-MMM-yyyy")}";
                noMatchesMessage = $"There are no ratings for species {speciesName} using scheme {schemeName}";
            }

            IEnumerable<SpeciesStatusRating> ratings = factory.SpeciesStatusRatings
                                                              .List(predicate, 1, int.MaxValue);
            if (ratings.Any())
            {
                output.WriteLine($"{title}:\n");
                SpeciesStatusRatingTable table = new SpeciesStatusRatingTable(ratings);
                table.PrintTable(output);
            }
            else
            {
                output.WriteLine(noMatchesMessage);
                output.Flush();
            }
        }
    }
}
