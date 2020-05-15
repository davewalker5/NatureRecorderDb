using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NatureRecorder.LegacyConverter.Entities;

namespace NatureRecorder.LegacyConverter.Logic
{
    public class DbReader
    {
        private const string ExpectedTitle = "Species Database";

        private readonly ListReader _reader = new ListReader();
        private readonly AppSettings _settings;

        public DbReader(AppSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Read the specified database
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="listFolder"></param>
        /// <returns></returns>
        public Database ReadDatabase(string filepath)
        {
            Database db;

            using (FileStream stream = new FileStream(filepath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    db = ReadHeader(reader, _settings.ListsFolder);
                    db.File = filepath;
                    db.Category = GetCategoryName(filepath);
                    ReadRecords(reader, db);
                }
            }

            return db;
        }

        /// <summary>
        /// Read the database header
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="listFolder"></param>
        /// <returns></returns>
        private Database ReadHeader(BinaryReader reader, string listFolder)
        {
            Database db = new Database();

            // The first 4 bytes are the version
            db.Version = reader.ReadInt32();

            // Followed by a fixed title string *but* as the original application
            // was written in C (at least at the data access level) we need to add
            // one character for the terminating NULL character

            // The title in at least one database file is corrupted and as we're not
            // using it in the way the original application did just read and throw
            // away the appropriate number of bytes
            // db.Title = ReadString(reader, ExpectedTitle.Length + 1);
            reader.ReadBytes(ExpectedTitle.Length + 1);

            // Followed by an 1024 character indexing species list name. This will
            // include the path and may be in uppercase so adjust the path and
            // convert to lowercase
            string speciesList = ReadFileName(reader, 1024);
            db.SpeciesList = Path.Combine(listFolder, speciesList);

            // Followed by a 1024 character indexing location list name
            string locationList = ReadFileName(reader, 1024);
            db.LocationList = Path.Combine(listFolder, locationList);

            return db;
        }

        /// <summary>
        /// Read the sightings from the database
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="db"></param>
        private void ReadRecords(BinaryReader reader, Database db)
        {
            // Use the list reader to read the species and location lists
            List<ListEntry> species = _reader.GetList(db.SpeciesList);
            List<ListEntry> locations = _reader.GetList(db.LocationList);

            // The remaining content are the content for this database
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {

                DbRecord record = new DbRecord { Category = db.Category };

                // LRN format           = Species index       (long integer)
                //                        Location index      (long integer)
                //                        Date as YYYYMMDD    (long integer)
                //                        Number recorded     (integer)
                //                        Record flags        (long integer)
                record.SpeciesId = reader.ReadInt32();
                record.LocationId = reader.ReadInt32();
                record.PackedDate = reader.ReadInt32();
                record.Number = reader.ReadInt16();
                record.Flags = reader.ReadInt32();

                // Convert the packed date into a DateTime type
                int year = record.PackedDate / 10000;
                int month = (record.PackedDate - 10000 * year) / 100;
                int day = record.PackedDate - 10000 * year - 100 * month;
                record.Date = new DateTime(year, month, day);

                // Look up the species and location tags
                record.Species = species.First(s => s.Index == record.SpeciesId).Tag;
                record.Location = locations.First(l => l.Index == record.LocationId).Tag;

                // Add this record to the database
                db.Records.Add(record);
            }
        }

        /// <summary>
        /// Get the category name given a database file name
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetCategoryName(string filePath)
        {
            // The species category is the database name without the path
            // and extension and with the leading digits, which denote the year
            // for the records contained in the file
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string category = Regex.Replace(fileName, @"[\d-]", string.Empty);
            return category;
        }

        /// <summary>
        /// Read a file name from the binary reader, removing the path if present
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        private string ReadFileName(BinaryReader reader, int characters)
        {
            string fileName;

            // Read the specified number of characters as the file path and
            // ensure required case
            string filePath = ReadString(reader, characters).ToLower();

            // The original application was an old Windows 95 based application
            // and the embedded paths are in a form that Path.GetFileName() doesn't
            // handle so resort to parsing out the file name manually by finding
            // the last index of the path separator character
            int lastSeparator = filePath.LastIndexOf(@"\");
            if (lastSeparator == -1)
            {
                // No separators. The path is the file name
                fileName = filePath;
            }
            else
            {
                // Filename starts after the final separator
                fileName = filePath.Substring(lastSeparator + 1);
            }

            return fileName;
        }

        /// <summary>
        /// Read a string from the binary reader and remove null characters
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        private string ReadString(BinaryReader reader, int characters)
        {
            return new string(reader.ReadChars(characters)).Replace("\0", "");
        }
    }
}
