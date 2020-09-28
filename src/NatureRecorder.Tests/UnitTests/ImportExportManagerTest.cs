using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class ImportExportManagerTest
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
        public void ImportTest()
        {
            int initialCount = _factory.Sightings.List(null, 1, int.MaxValue).Count();
            Assert.AreEqual(0, initialCount);

            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, sightings.Count());
            TestHelpers.ConfirmJackdawSighting(sightings);
            TestHelpers.ConfirmLapwingSighting(sightings);
        }

        [TestMethod]
        public void ExportTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            // Export it
            string exportFilePath = Path.GetTempFileName();
            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, int.MaxValue);
            _factory.Export.Export(sightings, exportFilePath);

            // Clear the database
            foreach (Sighting sighting in sightings)
            {
                _factory.Context.Sightings.Remove(sighting);
            }

            foreach (Location location in _factory.Locations.List(null, 1, int.MaxValue))
            {
                _factory.Context.Locations.Remove(location);
            }

            foreach (Species species in _factory.Species.List(null, 1, int.MaxValue))
            {
                _factory.Context.Species.Remove(species);
            }

            foreach (Category category in _factory.Categories.List(null, 1, int.MaxValue))
            {
                _factory.Context.Categories.Remove(category);
            }

            _factory.Context.SaveChanges();

            // Confirm the removal
            Assert.AreEqual(0, _factory.Context.Locations.Count());
            Assert.AreEqual(0, _factory.Context.Categories.Count());
            Assert.AreEqual(0, _factory.Context.Species.Count());
            Assert.AreEqual(0, _factory.Context.Sightings.Count());

            // Import the exported file and validate the import
            _factory.Import.Import(exportFilePath);
            File.Delete(exportFilePath);
            sightings = _factory.Sightings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, sightings.Count());
            TestHelpers.ConfirmJackdawSighting(sightings);
            TestHelpers.ConfirmLapwingSighting(sightings);
        }
    }
}
