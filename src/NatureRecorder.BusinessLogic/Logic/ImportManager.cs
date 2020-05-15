using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    public class ImportManager : IImportManager
    {
        private NatureRecorderFactory _factory;

        public event EventHandler<SightingDataExchangeEventArgs> RecordImport;

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
            using (StreamReader reader = new StreamReader(file))
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Read the CSV file and iterate over its contents
                    int count = 0;
                    IEnumerable<CsvSighting> records = csv.GetRecords<CsvSighting>();
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
            }
        }
    }
}
