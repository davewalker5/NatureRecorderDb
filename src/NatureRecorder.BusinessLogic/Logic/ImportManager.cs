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

namespace NatureRecorder.BusinessLogic.Logic
{
    public class ImportManager : IImportManager
    {
        private NatureRecorderFactory _factory;
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        public event EventHandler<SightingDataExchangeEventArgs> RecordImport;

        public IList<string> NewLocations { get; private set; }
        public IList<string> NewSpecies { get; private set; }
        public IList<string> NewCategories { get; private set; }

        public ImportManager(NatureRecorderFactory factory)
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
            IList<CsvSighting> records = Read(file);
            Save(records);
        }

        /// <summary>
        /// Detect new lookup data in the specified CSV file
        /// </summary>
        /// <param name="file"></param>
        public void DetectNewLookups(string file)
        {
            IList<CsvSighting> records = Read(file);
            NewLocations = GetNewLocations(records);
            NewCategories = GetNewCategories(records);
            NewSpecies = GetNewSpecies(records);
        }

        /// <summary>
        /// Write the details of lookup values to the console
        /// </summary>
        public void WriteNewLookupsToConsole()
        {
            WriteNewLookupsToConsole(NewLocations, "Locations");
            WriteNewLookupsToConsole(NewCategories, "Categories");
            WriteNewLookupsToConsole(NewSpecies, "Species");
        }

        /// <summary>
        /// Return a set of CSV records read from the specified CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private IList<CsvSighting> Read(string file)
        {
            IList<CsvSighting> records;

            using (StreamReader reader = new StreamReader(file))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                     records = csv.GetRecords<CsvSighting>().ToList();
                }
            }

            return records;
        }

        /// <summary>
        /// Save the specified collection of CSV records to the database
        /// </summary>
        /// <param name="records"></param>
        private void Save(IList<CsvSighting> records)
        {
            int count = 0;
            foreach (CsvSighting record in records)
            {
                // Map the current CSV record to a sighting and store it in the database
                Sighting sighting = Sighting.FromCsv(record);
                sighting = _factory.Sightings.Add(sighting);

                // Notify subscribers
                count++;
                RecordImport?.Invoke(this, new SightingDataExchangeEventArgs { RecordCount = count, Sighting = sighting });
            }
        }

        /// <summary>
        /// Identify new locations
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private IList<string> GetNewLocations(IList<CsvSighting> records)
        {
            IEnumerable<string> locations = records.Select(r => _textInfo.ToTitleCase(r.Location.CleanString()));
            IEnumerable<string> existing = _factory.Locations.List(null, 1, 99999999).Select(l => l.Name);
            return locations.Where(x => !existing.Contains(x)).ToList();
        }

        /// <summary>
        /// Identify new species
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private IList<string> GetNewSpecies(IList<CsvSighting> records)
        {
            IEnumerable<string> species = records.Select(r => _textInfo.ToTitleCase(r.Species.CleanString()));
            IEnumerable<string> existing = _factory.Species.List(null, 1, 99999999).Select(s => s.Name);
            return species.Where(x => !existing.Contains(x)).ToList();
        }

        /// <summary>
        /// Identify new categories
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private IList<string> GetNewCategories(IList<CsvSighting> records)
        {
            IEnumerable<string> categories = records.Select(r => _textInfo.ToTitleCase(r.Category.CleanString()));
            IEnumerable<string> existing = _factory.Categories.List(null, 1, 99999999).Select(s => s.Name);
            return categories.Where(x => !existing.Contains(x)).ToList();
        }

        /// <summary>
        /// Wrte a list of new lookups to the console
        /// </summary>
        /// <param name="values"></param>
        /// <param name="type"></param>
        private void WriteNewLookupsToConsole(IList<string> values, string type)
        {
            if (values.Any())
            {
                Console.WriteLine($"\nDetected the following new {type}:");
                foreach (string value in values)
                {
                    Console.WriteLine($"\t{value}");
                }
            }
            else
            {
                Console.WriteLine($"Detected no new {type}");
            }
        }
    }
}
