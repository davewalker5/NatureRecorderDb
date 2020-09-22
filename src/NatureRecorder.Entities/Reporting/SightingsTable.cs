using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Reporting
{
    public class SightingsTable
    {
        private IEnumerable<Sighting> _sightings = null;

        private const string IdHeader = "Id";
        private const string DateHeader = "Date";
        private const string LocationHeader = "Location";
        private const string SpeciesHeader = "Species";
        private const string CategoryHeader = "Category";
        private const string GenderHeader = "Gender";
        private const string NumberHeader = "Count";
        private const string WithYoungHeader = "With Young";

        private const string DateFormat = "dd-MMM-yyyy";
        private const int CellPadding = 1;
        private const char RowSeparator = '-';
        private const char ColumnSeparator = '|';

        private int _idColumnWidth;
        private int _dateColumnWidth;
        private int _locationColumnWidth;
        private int _speciesColumnWidth;
        private int _categoryColumnWidth;
        private int _genderColumnWidth;
        private int _numberColumnWidth;
        private int _withYoungColumnWidth;
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
            _idColumnWidth = Math.Max(IdHeader.Length, sightings.Max(s => s.Id.ToString().Length));
            _dateColumnWidth = DateTime.Now.ToString(DateFormat, null).Length;
            _locationColumnWidth = Math.Max(LocationHeader.Length, sightings.Max(s => s.Location.Name.Length));
            _speciesColumnWidth = Math.Max(SpeciesHeader.Length, sightings.Max(s => s.Species.Name.Length));
            _categoryColumnWidth = Math.Max(CategoryHeader.Length, sightings.Max(s => s.Species.Category.Name.Length));
            _genderColumnWidth = Math.Max(GenderHeader.Length, Gender.Unknown.ToString().Length);
            _numberColumnWidth = Math.Max(NumberHeader.Length, sightings.Max(s => s.Number.ToString().Length));
            _withYoungColumnWidth = Math.Max(WithYoungHeader.Length, 3);
        }

        /// <summary>
        /// Tabulate the sightings associated with this instance
        /// </summary>
        /// <param name="output"></param>
        public void PrintTable(StreamWriter output)
        {
            PrintRow(IdHeader, DateHeader, LocationHeader, SpeciesHeader, CategoryHeader, GenderHeader, NumberHeader, WithYoungHeader, false, output);
            PrintRow(RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), true, output);

            foreach (Sighting sighting in _sightings)
            {
                PrintRow(sighting, output);
            }

            output.Flush();
        }

        /// <summary>
        /// Print an individual table row using the specified sighting
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="output"></param>
        private void PrintRow(Sighting sighting, StreamWriter output)
        {
            PrintRow(sighting.Id.ToString($"D{_idColumnWidth}"),
                     sighting.Date.ToString(DateFormat),
                     sighting.Location.Name,
                     sighting.Species.Name,
                     sighting.Species.Category.Name,
                     sighting.Gender.ToString(),
                     (sighting.Number == 0) ? " " : sighting.Number.ToString(),
                     (sighting.WithYoung) ? "Yes" : "No",
                     false,
                     output);
        }

        /// <summary>
        /// Print a row of data
        /// </summary>
        /// <param name="date"></param>
        /// <param name="location"></param>
        /// <param name="species"></param>
        /// <param name="category"></param>
        /// <param name="gender"></param>
        /// <param name="number"></param>
        /// <param name="withYoung"></param>
        /// <param name="isSeparatorRow"></param>
        /// <param name="output"></param>
        private void PrintRow(string id, string date, string location, string species, string category, string gender, string number, string withYoung, bool isSeparatorRow, StreamWriter output)
        {
            output.Write(ColumnSeparator);
            PrintCell(_idColumnWidth, id, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_dateColumnWidth, date, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_locationColumnWidth, location, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_speciesColumnWidth, species, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_categoryColumnWidth, category, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_genderColumnWidth, gender, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_numberColumnWidth, number, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_withYoungColumnWidth, withYoung, isSeparatorRow, output);
            output.WriteLine(ColumnSeparator);
        }

        /// <summary>
        /// Print the content of a cell
        /// </summary>
        /// <param name="columnWidth"></param>
        /// <param name="value"></param>
        /// <param name="isSeparatorRow"></param>
        /// <param name="output"></param>
        private void PrintCell(int columnWidth, string value, bool isSeparatorRow, StreamWriter output)
        {
            char paddingCharacter = (isSeparatorRow) ? RowSeparator : ' ';
            output.Write($"{_padding}{value.PadRight(columnWidth, paddingCharacter)}{_padding}");
        }
    }
}
