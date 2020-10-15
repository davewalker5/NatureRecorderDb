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
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Interpreter.Logic;

namespace NatureRecorder.Tests.Helpers
{
    internal static class TestHelpers
    {
        private static string _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TestHelpers)).Location);

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
        /// Confirm the presence of a BOCC4 rating for the White-Fronted Goose
        /// in imported data
        /// </summary>
        /// <param name="sightings"></param>
        public static void ConfirmWhiteFrontedGooseRating(IEnumerable<SpeciesStatusRating> ratings)
        {
            SpeciesStatusRating rating = ratings.OrderBy(r => r.Start)
                                                .First(s => s.Species.Name == "White-Fronted Goose");
            Assert.AreEqual("Birds", rating.Species.Category.Name);
            Assert.AreEqual("BOCC4", rating.Rating.Scheme.Name);
            Assert.AreEqual("Amber", rating.Rating.Name);
            Assert.AreEqual("United Kingdom", rating.Region);
            Assert.AreEqual(new DateTime(2015, 12, 1, 0, 0, 0), rating.Start);
            Assert.AreEqual(new DateTime(2017, 12, 31, 0, 0, 0), rating.End);
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
        /// Run an export command to export data using the specified arguments
        /// </summary>
        /// <param name="exportFile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void ExportData(NatureRecorderFactory factory, string[] arguments)
        {
            RunCommand(factory, arguments, new ExportCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        /// <summary>
        /// Run a command and return the output as a string, optionally comparing
        /// it to a comparison file containing expected output. Input to the command
        /// is taken from an (optional) input file containing one entry per line
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="arguments"></param>
        /// <param name="command"></param>
        /// <param name="mode"></param>
        /// <param name="historyFile"></param>
        /// <param name="settingsFile"></param>
        /// <param name="inputFile"></param>
        /// <param name="comparisonFile"></param>
        /// <param name="skipLines"></param>
        /// <returns></returns>
        public static string RunCommand(NatureRecorderFactory factory,
                                        string[] arguments,
                                        CommandBase command,
                                        CommandMode mode,
                                        string historyFile,
                                        string settingsFile,
                                        string inputFile,
                                        string comparisonFile,
                                        int skipLines)
        {
            string data;
            StreamReader input = null;

            // Open the input file, if specified
            if (!string.IsNullOrEmpty(inputFile))
            {
                string commandFilePath = Path.Combine(_currentFolder, "Content", inputFile);
                input = new StreamReader(commandFilePath);
            }

            // Run the command, capturing the output
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    // Load user settings, if required
                    UserSettings settings = null;
                    if (!string.IsNullOrEmpty(settingsFile))
                    {
                        settings = new UserSettings(settingsFile);
                        settings.Load();
                    }

                    //  Run the command
                    command.Run(new CommandContext
                    {
                        Factory = factory,
                        Mode = mode,
                        Arguments = arguments ?? new string[] { },
                        Reader = (input != null) ? new StreamCommandReader(input) : null,
                        Output = output,
                        History = (!string.IsNullOrEmpty(historyFile)) ? new CommandHistory(historyFile) : null,
                        Settings = settings
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            // Close the input file
            if (input != null)
            {
                input.Dispose();
            }

            // Compare the output to the comparison file, if specified
            if (!string.IsNullOrEmpty(comparisonFile))
            {
                TestHelpers.CompareOutput(data, comparisonFile, skipLines);
            }

            return data;
        }
    }
}
