using System;

namespace NatureRecorder.LegacyConverter.Entities
{
    public class DbRecord
    {
        public int SpeciesId { get; set; }
        public string Species { get; set; }
        public string Category { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public int PackedDate { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public int Flags { get; set; }
        public static string CsvHeader { get { return "Species,Category,Number,Date,Location"; } }

        public string ToCsv()
        {
            return $"\"{Species}\",\"{Category}\",\"{Number}\",\"{Date.ToString("dd/MM/yyyy")}\",\"{Location}\"";
        }
    }
}
