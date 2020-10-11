using System;
using System.Collections.Generic;
using System.IO;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    public class SpeciesStatusExportManager : ISpeciesStatusExportManager
    {
        private readonly string[] ColumnHeaders =
        {
            "Species",
            "Category",
            "Scheme",
            "Rating",
            "Region",
            "Start",
            "End"
        };

        public event EventHandler<SpeciesStatusDataExchangeEventArgs> RecordExport;

        /// <summary>
        /// Export the specified collection of sightings to a CSV file
        /// </summary>
        /// <param name="ratings"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<SpeciesStatusRating> ratings, string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                string headers = string.Join(",", ColumnHeaders);
                writer.WriteLine(headers);

                int count = 0;
                foreach (SpeciesStatusRating rating in ratings)
                {
                    writer.WriteLine(rating.ToCsv());
                    count++;
                    RecordExport?.Invoke(this, new SpeciesStatusDataExchangeEventArgs { RecordCount = count, Rating = rating });
                }
            }
        }
    }
}
