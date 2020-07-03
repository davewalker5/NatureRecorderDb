using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NatureRecorder.Tests
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
    }
}
