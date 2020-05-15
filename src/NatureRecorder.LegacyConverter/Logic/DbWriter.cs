using System.IO;
using NatureRecorder.LegacyConverter.Entities;

namespace NatureRecorder.LegacyConverter.Logic
{
    public class DbWriter
    {
        /// <summary>
        /// Write the database to a CSV file
        /// </summary>
        /// <param name="db"></param>
        public void WriteDatabase(Database db)
        {
            string filepath = string.Concat(db.File, ".csv");
            using (StreamWriter writer = new StreamWriter(filepath, false))
            {
                writer.WriteLine(DbRecord.CsvHeader);
                foreach (DbRecord record in db.Records)
                {
                    writer.WriteLine(record.ToCsv());
                }
            }
        }
    }
}
