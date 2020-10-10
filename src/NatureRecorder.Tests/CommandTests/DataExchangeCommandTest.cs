using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Interpreter.Logic;
using NatureRecorder.Tests.Helpers;
using NatureRecorder.Tests.UnitTests;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class DataExchangeCommandTest
    {
        private NatureRecorderFactory _factory;
        private string _currentFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);
        }

        [TestMethod]
        public void CheckImportCommandTest()
        {
            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    string importFile = Path.Combine(_currentFolder, "Content", "valid-import.csv");
                    new CheckImportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { importFile }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "check-import.txt", 0);
        }

        [TestMethod]
        public void ImportCommandTest()
        {
            string commandFilePath = Path.Combine(_currentFolder, "Content", "import.txt");
            using (StreamReader input = new StreamReader(commandFilePath))
            {
                using (StreamWriter output = new StreamWriter(new MemoryStream()))
                {
                    string importFile = Path.Combine(_currentFolder, "Content", "valid-import.csv");
                    new ImportCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "sightings", importFile }
                    });
                }
            }

            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, sightings.Count());
            TestHelpers.ConfirmJackdawSighting(sightings);
            TestHelpers.ConfirmLapwingSighting(sightings);
        }

        [TestMethod]
        public void ExportAllTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[]
            {
                "all",
                Path.GetTempFileName()
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export.txt", arguments[1]);
            File.Delete(arguments[1]);
        }

        [TestMethod]
        public void ExportAllFromDateTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            string[] arguments = new string[]
            {
                "all",
                Path.GetTempFileName(),
                _factory.Sightings
                        .List(null, 1, int.MaxValue)
                        .OrderBy(s => s.Date)
                        .First()
                        .Date
                        .ToString("yyyy-MM-dd")
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export.txt", arguments[1]);
            File.Delete(arguments[1]);
        }

        [TestMethod]
        public void ExportAllFromToDateTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.SightingsImport.Import(importFilePath);

            IEnumerable<Sighting> sightings = _factory.Sightings
                                                      .List(null, 1, int.MaxValue)
                                                      .OrderBy(s => s.Date);

            string[] arguments = new string[]
            {
                "all",
                Path.GetTempFileName(),
                sightings.First().Date.ToString("yyyy-MM-dd"),
                sightings.Last().Date.ToString("yyyy-MM-dd")
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export.txt", arguments[1]);
            File.Delete(arguments[1]);
        }
    }
}
