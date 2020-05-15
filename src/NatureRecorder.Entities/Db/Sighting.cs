using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Sighting
    {
        private const string DateTimeFormat = "dd/MM/yyyy";

        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int SpeciesId { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }

        public Location Location { get; set; }
        public Species Species { get; set; }

        /// <summary>
        /// Return a CSV representation of this sighting
        /// </summary>
        /// <returns></returns>
        public string ToCsv()
        {
            return $"\"{Species.Name}\",\"{Species.Category.Name}\",\"{Number}\",\"{Date.ToString(DateTimeFormat)}\",\"{Location.Name}\"";
        }

        /// <summary>
        /// Given a CSV representation of a single sighting, expand it into a Sighting object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static Sighting FromCsv(CsvSighting record)
        {
            return new Sighting
            {
                Species = new Species
                {
                    Name = record.Species,
                    Category = new Category
                    {
                        Name = record.Category
                    }
                },
                Number = record.Number,
                Date = DateTime.ParseExact(record.Date, DateTimeFormat, CultureInfo.CurrentCulture),
                Location = new Location
                {
                    Name = record.Location
                }
            };
        }
    }
}
