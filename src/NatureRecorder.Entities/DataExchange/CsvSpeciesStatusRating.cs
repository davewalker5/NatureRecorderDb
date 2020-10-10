using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class CsvSpeciesStatusRating
    {
        public string Species { get; set; }
        public string Category { get; set; }
        public string Scheme { get; set; }
        public string Rating { get; set; }
        public string Region { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
