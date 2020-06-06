using System;
using System.Collections.Generic;
using System.Linq;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Reporting
{
    public class SightingsTable
    {
        private IEnumerable<Sighting> _sightings = null;

        private const string DateHeader = "Date";
        private const string LocationHeader = "Location";
        private const string SpeciesHeader = "Species";
        private const string CategoryHeader = "Category";

        private const string DateFormat = "dd-MMM-yyyy";
        private const int CellPadding = 1;
        private const char RowSeparator = '-';
        private const char ColumnSeparator = '|';

        private int _dateColumnWidth;
        private int _locationColumnWidth;
        private int _speciesColumnWidth;
        private int _categoryColumnWidth;
        private string _padding;

        /// <summary>
        /// Construct the table from the specified collection of sightings
        /// </summary>
        /// <param name="sightings"></param>
        public SightingsTable(IEnumerable<Sighting> sightings)
        {
            _sightings = sightings.OrderBy(s => s.Date)
                                  .ThenBy(s => s.Location.Name)
                                  .ThenBy(s => s.Species.Name);

            // Set the padding used either side of cell values
            _padding = new string(' ', CellPadding);

            // Set the column properties
            _dateColumnWidth = DateTime.Now.ToString(DateFormat, null).Length;
            _locationColumnWidth = Math.Max(LocationHeader.Length, sightings.Max(s => s.Location.Name.Length));
            _speciesColumnWidth = Math.Max(SpeciesHeader.Length, sightings.Max(s => s.Species.Name.Length));
            _categoryColumnWidth = Math.Max(CategoryHeader.Length, sightings.Max(s => s.Species.Category.Name.Length));
        }

        /// <summary>
        /// Tabulate the sightings associated with this instance
        /// </summary>
        public void PrintTable()
        {
            PrintRow(DateHeader, LocationHeader, SpeciesHeader, CategoryHeader, false);
            PrintRow(RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), true);

            foreach (Sighting sighting in _sightings)
            {
                PrintRow(sighting);
            }
        }

        /// <summary>
        /// Print an individual table row using the specified sighting
        /// </summary>
        /// <param name="sighting"></param>
        private void PrintRow(Sighting sighting)
        {
            PrintRow(sighting.Date.ToString(DateFormat),
                     sighting.Location.Name,
                     sighting.Species.Name,
                     sighting.Species.Category.Name,
                     false);
        }

        /// <summary>
        /// Print a row of data
        /// </summary>
        /// <param name="date"></param>
        /// <param name="location"></param>
        /// <param name="species"></param>
        /// <param name="category"></param>
        /// <param name="isSeparatorRow"></param>
        private void PrintRow(string date, string location, string species, string category, bool  isSeparatorRow)
        {
            Console.Write(ColumnSeparator);
            PrintCell(_dateColumnWidth, date, isSeparatorRow);
            Console.Write(ColumnSeparator);
            PrintCell(_locationColumnWidth, location, isSeparatorRow);
            Console.Write(ColumnSeparator);
            PrintCell(_speciesColumnWidth, species, isSeparatorRow);
            Console.Write(ColumnSeparator);
            PrintCell(_categoryColumnWidth, category, isSeparatorRow);
            Console.WriteLine(ColumnSeparator);
        }

        /// <summary>
        /// Print the content of a cell
        /// </summary>
        /// <param name="columnWidth"></param>
        /// <param name="value"></param>
        /// <param name="isSeparatorRow"></param>
        private void PrintCell(int columnWidth, string value, bool isSeparatorRow)
        {
            char paddingCharacter = (isSeparatorRow) ? RowSeparator : ' ';
            Console.Write($"{_padding}{value.PadRight(columnWidth, paddingCharacter)}{_padding}");
        }
    }
}
