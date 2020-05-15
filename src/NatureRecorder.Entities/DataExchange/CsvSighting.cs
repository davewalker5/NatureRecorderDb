using System;

namespace NatureRecorder.Entities.DataExchange
{
    public class CsvSighting
    {
        public string Species { get; set; }
        public string Category { get; set; }
        public int Number { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
    }
}
