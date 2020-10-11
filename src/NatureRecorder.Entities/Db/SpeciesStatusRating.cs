using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class SpeciesStatusRating : ExportableEntity
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

        /// <summary>
        /// Return a CSV representation of this sighting
        /// </summary>
        /// <returns></returns>
        public string ToCsv()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(MakeCsvField(Species.Name));
            builder.Append(MakeCsvField(Species.Category.Name));
            builder.Append(MakeCsvField(Rating.Scheme.Name));
            builder.Append(MakeCsvField(Rating.Name));
            builder.Append(MakeCsvField(Region));
            builder.Append(MakeCsvField(Start));
            builder.Append(MakeCsvField(End, true));
            return builder.ToString();
        }

        /// <summary>
        /// Given a CSV representation of a single sighting, expand it into a Sighting object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static SpeciesStatusRating FromCsv(CsvSpeciesStatusRating record)
        {
            return new SpeciesStatusRating
            {
                Species = new Species
                {
                    Name = record.Species,
                    Category = new Category
                    {
                        Name = record.Category
                    }
                },
                Rating = new StatusRating
                {
                    Name = record.Rating,
                    Scheme = new StatusScheme
                    {
                        Name = record.Scheme
                    }
                },
                Region = record.Region,
                Start = GetNullableDateTimeFromField(record.Start),
                End = GetNullableDateTimeFromField(record.End)
            };
        }
    }
}
