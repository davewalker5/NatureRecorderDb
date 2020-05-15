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
    public class CategoryManagerTest
    {
        private const string EntityName = "Birds";
        private const string AsyncEntityName = "Mammals";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _factory.Categories.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Categories.Add(EntityName);
            Assert.AreEqual(1, _factory.Categories.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Category entity = _factory.Categories.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Categories.AddAsync(AsyncEntityName);
            Category entity = await _factory.Categories.GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Category entity = _factory.Categories.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Category> entities = _factory.Categories.List(null, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Category> entities = await _factory.Categories
                                                    .ListAsync(null, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Category> entities = _factory.Categories.List(e => e.Name == EntityName, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Category> entities = await _factory.Categories
                                                    .ListAsync(e => e.Name == EntityName, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Category> entities = _factory.Categories.List(e => e.Name == "Missing", 1, 100);
            Assert.AreEqual(0, entities.Count());
        }
    }
}
