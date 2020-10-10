using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Reporting
{
    public class SpeciesStatusRatingTable
    {
        private const string DateFormat = "dd-MMM-yyyy HH:mm";

        private IEnumerable<SpeciesStatusRating> _ratings = null;

        private const string SpeciesHeader = "Species";
        private const string SchemeHeader = "Scheme";
        private const string RatingHeader = "Rating";
        private const string FromHeader = "From";
        private const string ToHeader = "To";

        private const int CellPadding = 1;
        private const char RowSeparator = '-';
        private const char ColumnSeparator = '|';

        private int _speciesColumnWidth;
        private int _schemeColumnWidth;
        private int _ratingColumnWidth;
        private int _fromColumnWidth;
        private int _toColumnWidth;
        private string _padding;

        /// <summary>
        /// Construct the table from the specified collection of species
        /// conservation status ratings
        /// </summary>
        /// <param name="ratings"></param>
        public SpeciesStatusRatingTable(IEnumerable<SpeciesStatusRating> ratings)
        {
            _ratings = ratings.OrderBy(r => r.Species)
                              .ThenBy(r => r.Rating.Scheme.Name)
                              .ThenBy(r => r.Start);

            // Set the padding used either side of cell values
            _padding = new string(' ', CellPadding);

            // Determine the column widths
            _speciesColumnWidth = Math.Max(SpeciesHeader.Length, ratings.Max(r => r.Species.Name.Length));
            _schemeColumnWidth = Math.Max(SchemeHeader.Length, ratings.Max(r => r.Rating.Scheme.Name.Length));
            _ratingColumnWidth = Math.Max(RatingHeader.Length, ratings.Max(r => r.Rating.Name.Length));
            _fromColumnWidth = Math.Max(FromHeader.Length, ratings.Max(r => GetDateCellValue(r.Start).Length));
            _toColumnWidth = Math.Max(ToHeader.Length, ratings.Max(r => GetDateCellValue(r.End).Length));
        }

        /// <summary>
        /// Tabulate the locations associated with this instance
        /// </summary>
        /// <param name="output"></param>
        public void PrintTable(StreamWriter output)
        {
            PrintRow(SpeciesHeader, SchemeHeader, RatingHeader, FromHeader, ToHeader, false, output);
            PrintRow(RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), true, output);

            foreach (SpeciesStatusRating rating in _ratings)
            {
                PrintRow(rating, output);
            }

            output.Flush();
        }

        /// <summary>
        /// Print an individual table row using the specified rating
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void PrintRow(SpeciesStatusRating rating, StreamWriter output)
        {
            PrintRow(rating.Species.Name,
                     rating.Rating.Scheme.Name,
                     rating.Rating.Name,
                     GetDateCellValue(rating.Start),
                     GetDateCellValue(rating.End),
                     false,
                     output);
        }

        /// <summary>
        /// Print a row of data
        /// </summary>
        /// <param name="species"></param>
        /// <param name="scheme"></param>
        /// <param name="rating"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isSeparatorRow"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void PrintRow(string species, string scheme, string rating, string from, string to, bool isSeparatorRow, StreamWriter output)
        {
            output.Write(ColumnSeparator);
            PrintCell(_speciesColumnWidth, species, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_schemeColumnWidth, scheme, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_ratingColumnWidth, rating, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_fromColumnWidth, from, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_toColumnWidth, to, isSeparatorRow, output);
            output.WriteLine(ColumnSeparator);
        }

        /// <summary>
        /// Print the content of a cell
        /// </summary>
        /// <param name="columnWidth"></param>
        /// <param name="value"></param>
        /// <param name="isSeparatorRow"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void PrintCell(int columnWidth, string value, bool isSeparatorRow, StreamWriter output)
        {
            char paddingCharacter = (isSeparatorRow) ? RowSeparator : ' ';
            output.Write($"{_padding}{value.PadRight(columnWidth, paddingCharacter)}{_padding}");
        }

        /// <summary>
        /// Convert a nullable date to a string in the required format for the table
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetDateCellValue(DateTime? date)
        {
            return (date == null) ? "" : (date ?? DateTime.Now).ToString(DateFormat);
        }
    }
}
