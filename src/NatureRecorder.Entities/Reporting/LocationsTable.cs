using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Reporting
{
    public class LocationsTable
    {
        private IEnumerable<Location> _locations = null;

        private const string LocationHeader = "Location";
        private const string AddressHeader = "Address";
        private const string LatitudeHeader = "Latitude";
        private const string LongitudeHeader = "Longitude";

        private const int CellPadding = 1;
        private const char RowSeparator = '-';
        private const char ColumnSeparator = '|';

        private int _locationColumnWidth;
        private int _addressColumnWidth;
        private int _latitudeColumnWidth;
        private int _longitudeColumnWidth;
        private string _padding;

        /// <summary>
        /// Construct the table from the specified collection of sightings
        /// </summary>
        /// <param name="locations"></param>
        public LocationsTable(IEnumerable<Location> locations)
        {
            _locations = locations.OrderBy(l => l.Name);

            // Set the padding used either side of cell values
            _padding = new string(' ', CellPadding);

            // Addresses are tabulated with the address cell containing all the components of the
            // address, on separate lines, so the width is the maximum across all those elements
            _addressColumnWidth = new int[]
                                        {
                                            AddressHeader.Length,
                                            locations.Max(l => l.Address?.Length) ?? 0,
                                            locations.Max(l => l.City?.Length) ?? 0,
                                            locations.Max(l => l.County?.Length) ?? 0,
                                            locations.Max(l => l.Postcode?.Length) ?? 0,
                                            locations.Max(l => l.Country?.Length) ?? 0
                                        }.Max(l => l);

            // Set the remaining properties
            _locationColumnWidth = Math.Max(LocationHeader.Length, locations.Max(l => l.Name.Length));
            _latitudeColumnWidth = Math.Max(LatitudeHeader.Length, locations.Max(s => s.Latitude.ToString().Length));
            _longitudeColumnWidth = Math.Max(LongitudeHeader.Length, locations.Max(s => s.Longitude.ToString().Length));
        }

        /// <summary>
        /// Tabulate the locations associated with this instance
        /// </summary>
        /// <param name="output"></param>
        public void PrintTable(StreamWriter output)
        {
            PrintRow(LocationHeader, AddressHeader, LatitudeHeader, LongitudeHeader, false, output);
            PrintRow(RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), RowSeparator.ToString(), true, output);

            foreach (Location location in _locations)
            {
                PrintRow(location, output);
            }

            output.Flush();
        }

        /// <summary>
        /// Print an individual table row using the specified sighting
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void PrintRow(Location location, StreamWriter output)
        {
            IList<string> lines = BuildAddressLines(location);
            for (int line = 0; line < lines.Count; line++)
            {
                if (line == 0)
                {
                    // The first line for this location includes all the location details
                    // and the first line of the address
                    PrintRow(location.Name, lines[line], location.Latitude.ToString(), location.Longitude.ToString(), false, output);
                }
                else
                {
                    // Subsequent lines only contain the corresponding line of the address
                    PrintRow(" ", lines[line], " ", " ", false, output);
                }
            }

            // End each location with a blank row
            PrintRow(" ", " ", " ", " ", false, output);
        }

        /// <summary>
        /// Print a row of data
        /// </summary>
        /// <param name="location"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isSeparatorRow"></param>
        /// <param name="output"></param>
        [ExcludeFromCodeCoverage]
        private void PrintRow(string location, string address, string latitude, string longitude, bool isSeparatorRow, StreamWriter output)
        {
            output.Write(ColumnSeparator);
            PrintCell(_locationColumnWidth, location, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_addressColumnWidth, address, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_latitudeColumnWidth, latitude, isSeparatorRow, output);
            output.Write(ColumnSeparator);
            PrintCell(_longitudeColumnWidth, longitude, isSeparatorRow, output);
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
        /// Return a list containing each non-null component of the location's address
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IList<string> BuildAddressLines(Location location)
        {
            // Build the list from the location's address components then filter
            // it to remove blank or empty lines
            List<string> address = new List<string>
                                    {
                                        location.Address,
                                        location.City,
                                        location.County,
                                        location.Postcode,
                                        location.Country
                                    }
                                    .Where(a => !string.IsNullOrEmpty(a))
                                    .ToList();

            // There must be at least one entry, blank if necessary
            if (!address.Any())
            {
                address.Add(" ");
            }

            return address;
        }
    }
}
