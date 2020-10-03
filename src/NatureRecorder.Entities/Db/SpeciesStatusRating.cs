using System;
using System.ComponentModel.DataAnnotations;

namespace NatureRecorder.Entities.Db
{
    public class SpeciesStatusRating
    {
        [Key]
        public int Id { get; set; }
        public int SpeciesId { get; set; }
        public int StatusRatingId { get; set; }
        public string Region { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public Species Species { get; set; }
        public StatusRating Rating { get; set; }
    }
}
