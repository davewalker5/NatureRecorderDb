using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Tests
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
            int initialCount = _factory.Sightings.List(null, 1, 100).Count();
            Assert.AreEqual(0, initialCount);

            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, 100);
            Assert.AreEqual(2, sightings.Count());
            ConfirmJackdawSighting(sightings);
            ConfirmLapwingSighting(sightings);
        }

        [TestMethod]
        public void ExportTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            // Export it
            string exportFilePath = Path.GetTempFileName();
            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, 100);
            _factory.Export.Export(sightings, exportFilePath);

            // Clear the database
            foreach (Sighting sighting in sightings)
            {
                _factory.Context.Sightings.Remove(sighting);
            }

            foreach (Location location in _factory.Locations.List(null, 1, 100))
            {
                _factory.Context.Locations.Remove(location);
            }

            foreach (Species species in _factory.Species.List(null, 1, 100))
            {
                _factory.Context.Species.Remove(species);
            }

            foreach (Category category in _factory.Categories.List(null, 1, 100))
            {
                _factory.Context.Categories.Remove(category);
            }

            _factory.Context.SaveChanges();

            // Confirm the removal
            Assert.AreEqual(0, _factory.Context.Locations.Count());
            Assert.AreEqual(0, _factory.Context.Categories.Count());
            Assert.AreEqual(0, _factory.Context.Species.Count());
            Assert.AreEqual(0, _factory.Context.Sightings.Count());

            // Import the exported file and validte the import
            _factory.Import.Import(exportFilePath);
            sightings = _factory.Sightings.List(null, 1, 100);
            Assert.AreEqual(2, sightings.Count());
            ConfirmJackdawSighting(sightings);
            ConfirmLapwingSighting(sightings);
        }

        private void ConfirmJackdawSighting(IEnumerable<Sighting> sightings)
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

        private void ConfirmLapwingSighting(IEnumerable<Sighting> sightings)
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
    }
}
