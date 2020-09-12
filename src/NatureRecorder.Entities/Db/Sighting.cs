﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
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
        public bool WithYoung { get; set; }

        public Location Location { get; set; }
        public Species Species { get; set; }

        /// <summary>
        /// Return a CSV representation of this sighting
        /// </summary>
        /// <returns></returns>
        public string ToCsv()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(MakeCsvField(Species.Name));
            builder.Append(MakeCsvField(Species.Category.Name));
            builder.Append(MakeCsvField(Number));
            builder.Append(MakeCsvField(WithYoung));
            builder.Append(MakeCsvField(Date));
            builder.Append(MakeCsvField(Location.Name));
            builder.Append(MakeCsvField(Location.Address));
            builder.Append(MakeCsvField(Location.City));
            builder.Append(MakeCsvField(Location.County));
            builder.Append(MakeCsvField(Location.Postcode));
            builder.Append(MakeCsvField(Location.Country));
            builder.Append(MakeCsvField(Location.Latitude));
            builder.Append(MakeCsvField(Location.Longitude, true));
            return builder.ToString();
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
                WithYoung = record.WithYoung,
                Date = DateTime.ParseExact(record.Date, DateTimeFormat, CultureInfo.CurrentCulture),
                Location = new Location
                {
                    Name = record.Location,
                    Address = record.Address,
                    City = record.City,
                    County = record.County,
                    Postcode = record.Postcode,
                    Country = record.Country,
                    Latitude = record.Latitude,
                    Longitude = record.Longitude
                }
            };
        }

        /// <summary>
        /// Create a CSV field value from a property value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLastColumn"></param>
        public string MakeCsvField(object value, bool isLastColumn = false)
        {
            string csvValue = "";

            if (value != null)
            {
                string type = value.GetType().Name;
                switch (type)
                {
                    case "DateTime":
                        csvValue = ((DateTime)value).ToString(DateTimeFormat);
                        break;
                    default:
                        csvValue = value.ToString();
                        break;
                }
            }

            string separator = (isLastColumn) ? "" : ",";
            return $"\"{csvValue}\"{separator}";
        }
    }
}
