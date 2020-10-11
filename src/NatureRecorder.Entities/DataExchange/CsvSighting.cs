using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class CsvSighting
    {
        public string Species { get; set; }
        public string Category { get; set; }
        public int Number { get; set; }
        public Gender Gender { get; set; }
        public bool WithYoung { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
