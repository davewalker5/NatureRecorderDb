using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Entities.Exceptions;
using System;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class LocationManagerTest
    {
        private const string EntityName = "Bagley Wood";
        private const string Address = "";
        private const string City = "Kennington";
        private const string County = "Oxfordshire";
        private const string Postcode = "";
        private const string Country = "United Kingdom";
        private const decimal Latitude = 51.7810M;
        private const decimal Longitude = 1.2611M;
        private const string AsyncEntityName = "College Lake";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _factory.Locations.Add(EntityName, Address, City, County, Postcode, Country, Latitude, Longitude);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Locations.Add(EntityName, "", "", "", "", "", null, null);
            Assert.AreEqual(1, _factory.Locations.List(null, 1, int.MaxValue).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Location entity = _factory.Locations.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
            Assert.AreEqual(Address, entity.Address);
            Assert.AreEqual(City, entity.City);
            Assert.AreEqual(County, entity.County);
            Assert.AreEqual(Postcode, entity.Postcode);
            Assert.AreEqual(Country, entity.Country);
            Assert.AreEqual(Latitude, entity.Latitude);
            Assert.AreEqual(Longitude, entity.Longitude);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Locations.AddAsync(AsyncEntityName, "", "", "", "", "", null, null);
            Location entity = await _factory.Locations.GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Location entity = _factory.Locations.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(null, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(null, 1, int.MaxValue)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(e => e.Name == EntityName, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(e => e.Name == EntityName, 1, int.MaxValue)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, entities.Count());
        }

        [TestMethod]
        public void DeleteTest()
        {
            _factory.Locations.Delete(EntityName);
            IEnumerable<Location> entities = _factory.Locations.List(null, 1, int.MaxValue);
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void DeleteMissingTest()
        {
            _factory.Locations.Delete("");
        }

        [TestMethod]
        [ExpectedException(typeof(LocationIsInUseException))]
        public void DeleteInUseTest()
        {
            Location entity = _factory.Locations.Get(a => a.Name == EntityName);
            Category category = _factory.Categories.Add("");
            Species species = _factory.Species.Add("", "");
            _factory.Sightings.Add(0, false, DateTime.Now, entity.Id, species.Id);
            _factory.Locations.Delete(EntityName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            await _factory.Locations.DeleteAsync(EntityName);
            IEnumerable<Location> entities = await _factory.Locations.ListAsync(null, 1, int.MaxValue).ToListAsync();
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public async Task DeleteMissingAsyncTest()
        {
            await _factory.Locations.DeleteAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(LocationIsInUseException))]
        public async Task DeleteInUseAsyncTest()
        {
            Location entity = await _factory.Locations.GetAsync(a => a.Name == EntityName);
            Category category = await _factory.Categories.AddAsync("");
            Species species = await _factory.Species.AddAsync("", "");
            await _factory.Sightings.AddAsync(0, false, DateTime.Now, entity.Id, species.Id);
            await _factory.Locations.DeleteAsync(EntityName);
        }
    }
}
