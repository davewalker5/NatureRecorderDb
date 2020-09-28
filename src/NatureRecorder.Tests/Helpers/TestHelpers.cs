using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.UnitTests;

namespace NatureRecorder.Tests.Helpers
{
    internal static class TestHelpers
    {
        private static string _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);

        /// <summary>
        /// Confirm the presence of a Jackdaw sighting in imported data
        /// </summary>
        /// <param name="sightings"></param>
        public static void ConfirmJackdawSighting(IEnumerable<Sighting> sightings)
        {
            Sighting sighting = sightings.First(s => s.Species.Name == "Jackdaw");
            Assert.AreEqual("Birds", sighting.Species.Category.Name);
            Assert.AreEqual(0, sighting.Number);
            Assert.AreEqual(new DateTime(1996, 11, 23), sighting.Date);
            Assert.AreEqual("Bagley Wood", sighting.Location.Name);
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.Address));
            Assert.AreEqual("Kennington", sighting.Location.City);
            Assert.AreEqual("Oxfordshire", sighting.Location.County);
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.Postcode));
            Assert.AreEqual("United Kingdom", sighting.Location.Country);
            Assert.AreEqual(51.781M, sighting.Location.Latitude);
            Assert.AreEqual(1.2611M, sighting.Location.Longitude);
        }

        /// <summary>
        /// Confirm the presence of a Lapwing sighting in imported data
        /// </summary>
        /// <param name="sightings"></param>
        public static void ConfirmLapwingSighting(IEnumerable<Sighting> sightings)
        {
            Sighting sighting = sightings.First(s => s.Species.Name == "Lapwing");
            Assert.AreEqual("Birds", sighting.Species.Category.Name);
            Assert.AreEqual(0, sighting.Number);
            Assert.AreEqual(new DateTime(2000, 1, 3), sighting.Date);
            Assert.AreEqual("College Lake", sighting.Location.Name);
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.Address));
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.City));
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.County));
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.Postcode));
            Assert.IsTrue(string.IsNullOrEmpty(sighting.Location.Country));
            Assert.IsNull(sighting.Location.Latitude);
            Assert.IsNull(sighting.Location.Longitude);
        }

        /// <summary>
        /// Read the specified memory stream's content to a string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadStream(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Compare the specified data with the contents of the specified test
        /// output verification file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        /// <param name="skipLines"></param>
        public static void CompareOutput(string data, string file, int skipLines)
        {
            // Split the test results data, ignoring empty entries
            string[] actualLines = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Read the verification file and split it, again ignoring empty
            // entries
            string filePath = Path.Combine(_currentFolder, "Content", file);
            string fileContent = File.ReadAllText(filePath);
            string[] expectedLines = fileContent.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(expectedLines.Length, actualLines.Length);
            for (int i = 0; i < expectedLines.Length; i++)
            {
                if ((i + 1) > skipLines)
                {
                    string expected = Regex.Replace(expectedLines[i], @"\s", "");
                    string actual = Regex.Replace(actualLines[i], @"\s", "");
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        /// Compare the content of two files
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="comparison"></param>
        public static void CompareFiles(string expected, string comparison)
        {
            string expectedFilePath = Path.Combine(_currentFolder, "Content", expected);
            string expectedContent = File.ReadAllText(expectedFilePath);
            string comparisonContent = File.ReadAllText(comparison);
            Assert.AreEqual(expectedContent, comparisonContent);
        }

        /// <summary>
        /// Run an export command using the specified arguments
        /// </summary>
        /// <param name="exportFile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void ExportData(NatureRecorderFactory factory, string[] arguments)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ExportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = factory,
                        Mode = CommandMode.Interactive,
                        Arguments = arguments
                    });
                }
            }
        }
    }
}
