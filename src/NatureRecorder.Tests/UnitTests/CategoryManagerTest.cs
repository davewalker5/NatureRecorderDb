using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Entities.Exceptions;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class CategoryManagerTest
    {
        private const string EntityName = "Birds";
        private const string AsyncEntityName = "Mammals";
        private const string RenamedEntityName = "Marsupials";

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
            Assert.AreEqual(1, _factory.Categories.List(null, 1, int.MaxValue).Count());
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
            IEnumerable<Category> entities = _factory.Categories.List(null, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Category> entities = await _factory.Categories
                                                    .ListAsync(null, 1, int.MaxValue)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Category> entities = _factory.Categories.List(e => e.Name == EntityName, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Category> entities = await _factory.Categories
                                                    .ListAsync(e => e.Name == EntityName, 1, int.MaxValue)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Category> entities = _factory.Categories.List(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, entities.Count());
        }

        [TestMethod]
        public void RenameTest()
        {
            Category category = _factory.Categories.Rename(EntityName, RenamedEntityName);
            Assert.AreEqual(category.Name, RenamedEntityName);

            Category original = _factory.Categories.Get(s => s.Name == EntityName);
            Assert.IsNull(original);
        }

        [TestMethod]
        public async Task RenameAsyncTest()
        {
            Category category = await _factory.Categories.RenameAsync(EntityName, RenamedEntityName);
            Assert.AreEqual(category.Name, RenamedEntityName);

            Category original = await _factory.Categories.GetAsync(s => s.Name == EntityName);
            Assert.IsNull(original);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void RenameMissingTest()
        {
            _factory.Categories.Rename("", RenamedEntityName);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public async Task RenameMissingAsyncTest()
        {
            await _factory.Categories.RenameAsync("", RenamedEntityName);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryAlreadyExistsException))]
        public void RenameToExistingTest()
        {
            _factory.Categories.Rename(EntityName, EntityName);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryAlreadyExistsException))]
        public async Task RenameToExistingAsyncTest()
        {
            await _factory.Categories.RenameAsync(EntityName, EntityName);
        }

        [TestMethod]
        public void DeleteTest()
        {
            _factory.Categories.Delete(EntityName);
            IEnumerable<Category> entities = _factory.Categories.List(null, 1, int.MaxValue);
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void DeleteMissingTest()
        {
            _factory.Categories.Delete("");
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryIsInUseException))]
        public void DeleteInUseTest()
        {
            Category entity = _factory.Categories.Get(a => a.Name == EntityName);
            Species species = _factory.Species.Add("", EntityName);
            _factory.Categories.Delete(EntityName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            await _factory.Categories.DeleteAsync(EntityName);
            IEnumerable<Category> entities = await _factory.Categories.ListAsync(null, 1, int.MaxValue).ToListAsync();
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public async Task DeleteMissingAsyncTest()
        {
            await _factory.Categories.DeleteAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryIsInUseException))]
        public async Task DeleteInUseAsyncTest()
        {
            Category entity = await _factory.Categories.GetAsync(a => a.Name == EntityName);
            Species species = await _factory.Species.AddAsync("", EntityName);
            await _factory.Categories.DeleteAsync(EntityName);
        }
    }
}
