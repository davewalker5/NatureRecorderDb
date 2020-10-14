using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
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
        public void CheckSightingsImportCommandTest()
        {
            string importFile = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            string[] arguments = new string[] { "sightings", importFile };
            TestHelpers.RunCommand(_factory, arguments, new CheckImportCommand(), CommandMode.CommandLine, null, null, null, "check-import.txt", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownDataExchangeTypeException))]
        public void CheckInvalidDataExchangeType()
        {
            string[] arguments = new string[] { "invalid", "filename" };
            TestHelpers.RunCommand(_factory, arguments, new CheckImportCommand(), CommandMode.CommandLine, null, null, null,  null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownDataExchangeTypeException))]
        public void ImportInvalidDataExchangeType()
        {
            string[] arguments = new string[] { "invalid", "filename" };
            TestHelpers.RunCommand(_factory, arguments, new ImportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownDataExchangeTypeException))]
        public void ExportInvalidDataExchangeType()
        {
            string[] arguments = new string[] { "invalid", "filename" };
            TestHelpers.RunCommand(_factory, arguments, new ExportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
        }

        [TestMethod]
        public void ImportSightingsCommandTest()
        {
            string importFile = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            string[] arguments = new string[] { "sightings", importFile };
            TestHelpers.RunCommand(_factory, arguments, new ImportCommand(), CommandMode.CommandLine, null, null, "import.txt", null, 0);

            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, sightings.Count());
            TestHelpers.ConfirmJackdawSighting(sightings);
            TestHelpers.ConfirmLapwingSighting(sightings);
        }

        [TestMethod]
        public void ExportAllSightingsTest()
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
        public void ExportAllSightingsFromDateTest()
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
        public void ExportAllSightingsFromToDateTest()
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

        [TestMethod]
        public void CheckStatusImportCommandTest()
        {
            string importFile = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            string[] arguments = new string[] { "status", importFile };
            TestHelpers.RunCommand(_factory, arguments, new CheckImportCommand(), CommandMode.CommandLine, null, null, null, "check-status-import.txt", 0);
        }

        [TestMethod]
        public void ImportStatusCommandTest()
        {
            string importFile = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            string[] arguments = new string[] { "status", importFile };
            TestHelpers.RunCommand(_factory, arguments, new ImportCommand(), CommandMode.CommandLine, null, null, "import.txt", null, 0);

            IEnumerable<SpeciesStatusRating> ratings = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, ratings.Count());
            TestHelpers.ConfirmWhiteFrontedGooseRating(ratings);
        }

        [TestMethod]
        public void ExportAllStatusesTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[]
            {
                "status",
                "all",
                Path.GetTempFileName()
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export-status.txt", arguments[2]);
            File.Delete(arguments[1]);
        }

        [TestMethod]
        public void ExportStatusSchemeTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[]
            {
                "status",
                "BOCC4",
                Path.GetTempFileName()
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export-status.txt", arguments[2]);
            File.Delete(arguments[1]);
        }

        [TestMethod]
        public void ExportStatusSchemeAtDateTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            string[] arguments = new string[]
            {
                "status",
                "BOCC4",
                Path.GetTempFileName()
            };

            TestHelpers.ExportData(_factory, arguments);
            TestHelpers.CompareFiles("export-status.txt", arguments[2]);
            File.Delete(arguments[1]);
        }
    }
}
