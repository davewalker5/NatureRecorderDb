using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Reporting;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class SightingsManagerTest
    {
        private const string LocationName = "Bagley Wood";
        private const string Address = "";
        private const string City = "Kennington";
        private const string County = "Oxfordshire";
        private const string Postcode = "";
        private const string Country = "United Kingdom";
        private const decimal Latitude = 51.7810M;
        private const decimal Longitude = 1.2611M;
        private const string CategoryName = "Birds";
        private const string SpeciesName = "Red Kite";

        private NatureRecorderFactory _factory;
        private int _sightingId;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            Location location = _factory.Locations.Add(LocationName, Address, City, County, Postcode, Country, Latitude, Longitude);
            _factory.Categories.Add(CategoryName);
            Species species = _factory.Species.Add(SpeciesName, CategoryName);
            _sightingId = _factory.Sightings.Add(0, Gender.Unknown, false, DateTime.Now, location.Id, species.Id).Id;
        }

        [TestMethod]
        public void GetTest()
        {
            Sighting sighting = _factory.Sightings.Get(s => s.Id == _sightingId);
            Assert.IsNotNull(sighting);
        }

        [TestMethod]
        public async Task GetAsyncTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(s => s.Id == _sightingId);
            Assert.IsNotNull(sighting);
        }

        [TestMethod]
        public void ListByLocationTest()
        {
            int locationId = _factory.Locations.List(null, 1, int.MaxValue).First().Id;
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByLocation(locationId, 1, int.MaxValue);
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public async Task ListByLocationAsyncTest()
        {
            Location location = await _factory.Locations.ListAsync(null, 1, int.MaxValue).FirstAsync();
            List<Sighting> sightings = await _factory.Sightings.ListByLocationAsync(location.Id, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public void ListByCategoryTest()
        {
            int categoryId = _factory.Categories.List(null, 1, int.MaxValue).First().Id;
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByCategory(categoryId, 1, int.MaxValue);
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public async Task ListByCategoryAsyncTest()
        {
            Category category = await _factory.Categories.ListAsync(null, 1, int.MaxValue).FirstAsync();
            List<Sighting> sightings = await _factory.Sightings.ListByCategoryAsync(category.Id, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public void ListBySpeciesTest()
        {
            int speciesId = _factory.Species.List(null, 1, int.MaxValue).First().Id;
            IEnumerable<Sighting> sightings = _factory.Sightings.ListBySpecies(speciesId, 1, int.MaxValue);
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public async Task ListBySpeciesAsyncTest()
        {
            Species species = await _factory.Species.ListAsync(null, 1, int.MaxValue).FirstAsync();
            List<Sighting> sightings = await _factory.Sightings.ListBySpeciesAsync(species.Id, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public void ListByDateTest()
        {
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByDate(from, to, 1, int.MaxValue);
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public async Task ListByDateAsyncTest()
        {
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            List<Sighting> sightings = await _factory.Sightings.ListByDateAsync(from, to, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, sightings.Count());
        }

        [TestMethod]
        public void SummariseTest()
        {
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            int locationId = _factory.Locations.List(null, 1, int.MaxValue).First().Id;
            int categoryId = _factory.Categories.List(null, 1, int.MaxValue).First().Id;
            int speciesId = _factory.Species.List(null, 1, int.MaxValue).First().Id;

            Summary summary =_factory.Sightings.Summarise(from, to, locationId, categoryId, speciesId);

            Assert.AreEqual(1, summary.Locations.Count());
            Assert.AreEqual(1, summary.Categories.Count());
            Assert.AreEqual(1, summary.Species.Count());
            Assert.AreEqual(1, summary.Sightings.Count());
        }

        [TestMethod]
        public async Task SummariseAsyncTest()
        {
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            Location location = await _factory.Locations.ListAsync(null, 1, int.MaxValue).FirstAsync();
            Category category = await _factory.Categories.ListAsync(null, 1, int.MaxValue).FirstAsync();
            Species species = await _factory.Species.ListAsync(null, 1, int.MaxValue).FirstAsync();

            Summary summary = await _factory.Sightings.SummariseAsync(from, to, location.Id, category.Id, species.Id);

            Assert.AreEqual(1, summary.Locations.Count());
            Assert.AreEqual(1, summary.Categories.Count());
            Assert.AreEqual(1, summary.Species.Count());
            Assert.AreEqual(1, summary.Sightings.Count());
        }

        [TestMethod]
        public void DeleteTest()
        {
            int sightingId = _factory.Sightings.List(null, 1, int.MaxValue).First().Id;
            _factory.Sightings.Delete(sightingId);
            Assert.AreEqual(0, _factory.Context.Sightings.Count());
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            Sighting sighting = await _factory.Sightings.ListAsync(null, 1, int.MaxValue).FirstAsync();
            await _factory.Sightings.DeleteAsync(sighting.Id);
            Assert.AreEqual(0, _factory.Context.Sightings.Count());
        }
    }
}
