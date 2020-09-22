using System;
using System.Collections.Generic;
using System.IO;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class ExportManager : IExportManager
    {
        private readonly string[] ColumnHeaders =
        {
            "Species",
            "Category",
            "Number",
            "Gender",
            "WithYoung",
            "Date",
            "Location",
            "Address",
            "City",
            "County",
            "Postcode",
            "Country",
            "Latitude",
            "Longitude"
        };

        public event EventHandler<SightingDataExchangeEventArgs> RecordExport;

        /// <summary>
        /// Export the specified collection of sightings to a CSV file
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<Sighting> sightings, string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                string headers = string.Join(",", ColumnHeaders);
                writer.WriteLine(headers);

                int count = 0;
                foreach (Sighting sighting in sightings)
                {
                    writer.WriteLine(sighting.ToCsv());
                    count++;
                    RecordExport?.Invoke(this, new SightingDataExchangeEventArgs { RecordCount = count, Sighting = sighting });
                }
            }
        }
    }
}
