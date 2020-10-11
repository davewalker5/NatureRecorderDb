using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Interfaces;
using NatureRecorder.BusinessLogic.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.BusinessLogic.Logic
{
    public class SpeciesStatusImportManager : ISpeciesStatusImportManager
    {
        private NatureRecorderFactory _factory;
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        public event EventHandler<SpeciesStatusDataExchangeEventArgs> RecordImport;

        public IList<string> NewSpecies { get; private set; }
        public IList<string> NewCategories { get; private set; }
        public IList<string> NewSchemes { get; private set; }
        public IList<string> NewRatings { get; private set; }

        public SpeciesStatusImportManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Import the contents of the CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        public void Import(string file)
        {
            IList<CsvSpeciesStatusRating> records = Read(file);
            Save(records);
        }

        /// <summary>
        /// Detect new lookup data in the specified CSV file
        /// </summary>
        /// <param name="file"></param>
        public void DetectNewLookups(string file)
        {
            IList<CsvSpeciesStatusRating> records = Read(file);
            NewCategories = GetNewCategories(records);
            NewSpecies = GetNewSpecies(records);
            NewSchemes = GetNewSchemes(records);
            NewRatings = GetNewRatings(records, NewSchemes);
        }

        /// <summary>
        /// Write the details of lookup values to the console
        /// </summary>
        /// <param name="output"></param>
        public void WriteNewLookupsToStream(StreamWriter output)
        {
            WriteNewLookupsToStream(NewCategories, "Categories", output);
            WriteNewLookupsToStream(NewSpecies, "Species", output);
            WriteNewLookupsToStream(NewSchemes, "Conservation Status Schemes", output);
            WriteNewLookupsToStream(NewRatings, "Conservation Status Ratings", output);
        }

        /// <summary>
        /// Return a set of CSV records read from the specified CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<CsvSpeciesStatusRating> Read(string file)
        {
            IList<CsvSpeciesStatusRating> records;

            using (StreamReader reader = new StreamReader(file))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<CsvSpeciesStatusRating>().ToList();
                }
            }

            return records;
        }

        /// <summary>
        /// Save the specified collection of CSV records to the database
        /// </summary>
        /// <param name="records"></param>
        [ExcludeFromCodeCoverage]
        private void Save(IList<CsvSpeciesStatusRating> records)
        {
            int count = 0;
            foreach (CsvSpeciesStatusRating record in records)
            {
                // Map the current CSV record to a conservation status rating, check it
                // doesn't already exist and store it in the database
                SpeciesStatusRating rating = SpeciesStatusRating.FromCsv(record);
                SpeciesStatusRating existing = _factory.Context
                                                       .SpeciesStatusRatings
                                                       .FirstOrDefault(r => (r.Species.Name == rating.Species.Name) &&
                                                                            (r.Rating.Name == rating.Rating.Name) &&
                                                                            (r.Region == rating.Region) &&
                                                                            ((r.Start == rating.Start) || ((r.Start == null) && (rating.Start == null))) &&
                                                                            ((r.End == rating.End) || ((r.End == null) && (rating.End == null))));
                if (existing == null)
                {
                    rating = _factory.SpeciesStatusRatings.Add(rating);
                }

                // Notify subscribers
                count++;
                RecordImport?.Invoke(this, new SpeciesStatusDataExchangeEventArgs { RecordCount = count, Rating = rating });
            }
        }

        /// <summary>
        /// Identify new species
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<string> GetNewSpecies(IList<CsvSpeciesStatusRating> records)
        {
            IEnumerable<string> species = records.Select(r => _textInfo.ToTitleCase(r.Species.CleanString()));
            IEnumerable<string> existing = _factory.Species.List(null, 1, int.MaxValue).Select(s => s.Name);
            return species.Where(x => !existing.Contains(x))
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Identify new categories
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<string> GetNewCategories(IList<CsvSpeciesStatusRating> records)
        {
            IEnumerable<string> categories = records.Select(r => _textInfo.ToTitleCase(r.Category.CleanString()));
            IEnumerable<string> existing = _factory.Categories.List(null, 1, int.MaxValue).Select(s => s.Name);
            return categories.Where(x => !existing.Contains(x))
                             .Distinct()
                             .ToList();
        }

        /// <summary>
        /// Identify new conservation status schemes
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<string> GetNewSchemes(IList<CsvSpeciesStatusRating> records)
        {
            IEnumerable<string> schemes = records.Select(r => _textInfo.ToTitleCase(r.Scheme.CleanString()));
            IEnumerable<string> existing = _factory.StatusSchemes.List(null, 1, int.MaxValue).Select(s => s.Name);
            return schemes.Where(x => !existing.Contains(x))
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Identify new conservation status ratings
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<string> GetNewRatings(IList<CsvSpeciesStatusRating> records, IList<string> newSchemes)
        {
            List<string> newRatings = new List<string>();

            // Get a distinct list of schemes in the input file
            IEnumerable<string> schemes = records.Select(r => _textInfo.ToTitleCase(r.Scheme.CleanString()))
                                                 .Where(r => !newSchemes.Contains(r))
                                                 .Distinct();
            foreach (string scheme in schemes)
            {
                // For each scheme, identify the existing ratings
                IEnumerable<string> existing = _factory.StatusRatings.List(r => r.Scheme.Name == scheme, 1, int.MaxValue).Select(r => r.Name);

                // Find new ratings listed in the file
                IEnumerable<string> newRatingsForScheme = records.Where(r => (_textInfo.ToTitleCase(r.Scheme.CleanString()) == scheme) &&
                                                                             !existing.Contains(_textInfo.ToTitleCase(r.Rating.CleanString())))
                                                                 .Select(r => $"{_textInfo.ToTitleCase(r.Rating.CleanString())} (scheme {scheme})");
                newRatings.AddRange(newRatingsForScheme);
            }

            return newRatings;
        }

        /// <summary>
        /// Wrte a list of new lookups to the console
        /// </summary>
        /// <param name="values"></param>
        /// <param name="type"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void WriteNewLookupsToStream(IList<string> values, string type, StreamWriter output)
        {
            if (values.Any())
            {
                output.WriteLine($"\nDetected the following new {type}:");
                foreach (string value in values)
                {
                    output.WriteLine($"\t{value}");
                }
            }
            else
            {
                output.WriteLine($"Detected no new {type}");
            }
        }
    }
}
