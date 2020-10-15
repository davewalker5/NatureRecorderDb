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
    public class StatusImportExportManagerTest
    {
        private NatureRecorderFactory _factory;
        private string _currentFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(StatusImportExportManagerTest)).Location);
        }

        [TestMethod]
        public void ImportTest()
        {
            int initialCount = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue).Count();
            Assert.AreEqual(0, initialCount);

            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            IEnumerable<SpeciesStatusRating> ratings = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, ratings.Count());
            TestHelpers.ConfirmWhiteFrontedGooseRating(ratings);
        }

        [TestMethod]
        public void ImportForExistingEntitiesTest()
        {
            _factory.StatusSchemes.Add("BOCC4");
            _factory.StatusRatings.Add("red", "BOCC4");
            _factory.StatusRatings.Add("amber", "BOCC4");
            _factory.Categories.Add("birds");
            _factory.Species.Add("white-fronted goose", "birds");

            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            Assert.AreEqual(1, _factory.Context.StatusSchemes.Count());
            Assert.AreEqual(2, _factory.Context.StatusRatings.Count());
            Assert.AreEqual(1, _factory.Context.Categories.Count());
            Assert.AreEqual(1, _factory.Context.Species.Count());

            IEnumerable<SpeciesStatusRating> ratings = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, ratings.Count());
            TestHelpers.ConfirmWhiteFrontedGooseRating(ratings);
        }

        [TestMethod]
        public void ExportTest()
        {
            // Set up the data to export
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-status-import.csv");
            _factory.SpeciesStatusImport.Import(importFilePath);

            // Export it
            string exportFilePath = Path.GetTempFileName();
            IEnumerable<SpeciesStatusRating> ratings = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            _factory.SpeciesStatusExport.Export(ratings, exportFilePath);

            // Clear the database
            foreach (SpeciesStatusRating rating in ratings)
            {
                _factory.Context.SpeciesStatusRatings.Remove(rating);
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
            Assert.AreEqual(0, _factory.Context.SpeciesStatusRatings.Count());
            Assert.AreEqual(0, _factory.Context.Categories.Count());
            Assert.AreEqual(0, _factory.Context.Species.Count());

            // Import the exported file and validate the import
            _factory.SpeciesStatusImport.Import(exportFilePath);
            File.Delete(exportFilePath);
            ratings = _factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            Assert.AreEqual(2, ratings.Count());
            TestHelpers.ConfirmWhiteFrontedGooseRating(ratings);
        }
    }
}
