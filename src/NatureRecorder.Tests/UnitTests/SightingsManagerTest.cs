using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;

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
        public void GetByIdTest()
        {
            Sighting sighting = _factory.Sightings.Get(s => s.Id == _sightingId);
            Assert.IsNotNull(sighting);
        }
    }
}
