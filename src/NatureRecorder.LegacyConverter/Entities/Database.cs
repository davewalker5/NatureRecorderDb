using System.Collections.Generic;

namespace NatureRecorder.LegacyConverter.Entities
{
    public class Database
    {
        public string File { get; set; }
        public int Version { get; set; }
        public string Title { get; set; }
        public string SpeciesList { get; set; }
        public string LocationList { get; set; }
        public string Category { get; set; }
        public List<DbRecord> Records { get; private set; }

        public Database()
        {
            Records = new List<DbRecord>();
        }
    }
}
