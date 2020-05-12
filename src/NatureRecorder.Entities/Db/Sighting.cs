using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Sighting
    {
        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int SpeciesId { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }

        public Location Location { get; set; }
        public Species Species { get; set; }
    }
}
