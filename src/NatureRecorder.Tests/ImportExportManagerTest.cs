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
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual("Jackdaw", sightings.First().Species.Name);
            Assert.AreEqual("Birds", sightings.First().Species.Category.Name);
            Assert.AreEqual(0, sightings.First().Number);
            Assert.AreEqual(new DateTime(1996, 11, 23), sightings.First().Date);
            Assert.AreEqual("Bagley Wood", sightings.First().Location.Name);
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
            Sighting sighting = _factory.Context.Sightings.First();
            _factory.Context.Sightings.Remove(sighting);

            Location location = _factory.Locations.Get(l => l.Id == sighting.LocationId);
            _factory.Context.Locations.Remove(location);

            Species species = _factory.Species.Get(s => s.Id == sighting.SpeciesId);
            _factory.Context.Species.Remove(species);

            Category category = _factory.Categories.Get(c => c.Id == sighting.Species.Category.Id);
            _factory.Context.Categories.Remove(category);
            _factory.Context.SaveChanges();

            // Confirm the removal
            Assert.AreEqual(0, _factory.Context.Locations.Count());
            Assert.AreEqual(0, _factory.Context.Categories.Count());
            Assert.AreEqual(0, _factory.Context.Species.Count());
            Assert.AreEqual(0, _factory.Context.Sightings.Count());

            // Import the exported file and validte the import
            _factory.Import.Import(exportFilePath);
            sightings = _factory.Sightings.List(null, 1, 100);
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual("Jackdaw", sightings.First().Species.Name);
            Assert.AreEqual("Birds", sightings.First().Species.Category.Name);
            Assert.AreEqual(0, sightings.First().Number);
            Assert.AreEqual(new DateTime(1996, 11, 23), sightings.First().Date);
            Assert.AreEqual("Bagley Wood", sightings.First().Location.Name);
        }
    }
}
