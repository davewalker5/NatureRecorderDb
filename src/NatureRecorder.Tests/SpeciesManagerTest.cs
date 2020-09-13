using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Entities.Exceptions;

namespace NatureRecorder.Tests
{
    [TestClass]
    public class SpeciesManagerTest
    {
        private const string SpeciesName = "Red Kite";
        private const string AsyncSpeciesName = "Blackbird";
        private const string RenameSpeciesName = "Robin";
        private const string CategoryName = "Birds";
        private const string MoveCategoryName = "Things With Wings";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _factory.Species.Add(SpeciesName, CategoryName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Species.Add(SpeciesName, CategoryName);
            Assert.AreEqual(1, _factory.Species.List(null, 1, int.MaxValue).Count());
            Assert.AreEqual(1, _factory.Categories.List(null, 1, int.MaxValue).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Species species = _factory.Species.Get(a => a.Name == SpeciesName);

            Assert.IsNotNull(species);
            Assert.IsTrue(species.Id > 0);
            Assert.AreEqual(SpeciesName, species.Name);

            Assert.IsNotNull(species.Category);
            Assert.IsTrue(species.Category.Id > 0);
            Assert.AreEqual(CategoryName, species.Category.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Species.AddAsync(AsyncSpeciesName, CategoryName);
            Species species = await _factory.Species.GetAsync(a => a.Name == AsyncSpeciesName);

            Assert.IsNotNull(species);
            Assert.IsTrue(species.Id > 0);
            Assert.AreEqual(AsyncSpeciesName, species.Name);

            Assert.IsNotNull(species.Category);
            Assert.IsTrue(species.Category.Id > 0);
            Assert.AreEqual(CategoryName, species.Category.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Species species = _factory.Species.Get(a => a.Name == "Missing");
            Assert.IsNull(species);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Species> species = _factory.Species.List(null, 1, int.MaxValue);
            Assert.AreEqual(1, species.Count());
            Assert.AreEqual(SpeciesName, species.First().Name);
            Assert.AreEqual(CategoryName, species.First().Category.Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Species> species = await _factory.Species
                                                  .ListAsync(null, 1, int.MaxValue)
                                                  .ToListAsync();
            Assert.AreEqual(1, species.Count());
            Assert.AreEqual(SpeciesName, species.First().Name);
            Assert.AreEqual(CategoryName, species.First().Category.Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Species> species = _factory.Species
                                                   .List(e => e.Name == SpeciesName, 1, int.MaxValue);
            Assert.AreEqual(1, species.Count());
            Assert.AreEqual(SpeciesName, species.First().Name);
            Assert.AreEqual(CategoryName, species.First().Category.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Species> models = await _factory.Species
                                                 .ListAsync(e => e.Name == SpeciesName, 1, int.MaxValue)
                                                 .ToListAsync();
            Assert.AreEqual(1, models.Count());
            Assert.AreEqual(SpeciesName, models.First().Name);
            Assert.AreEqual(CategoryName, models.First().Category.Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Species> models = _factory.Species.List(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, models.Count());
        }

        [TestMethod]
        public void ListByCategoryTest()
        {
            IEnumerable<Species> species = _factory.Species.ListByCategory(CategoryName, 1, int.MaxValue);
            Assert.AreEqual(1, species.Count());
            Assert.AreEqual(SpeciesName, species.First().Name);
            Assert.AreEqual(CategoryName, species.First().Category.Name);
        }

        [TestMethod]
        public async Task ListByCateoryAsyncTest()
        {
            List<Species> species = await _factory.Species
                                                 .ListByCategoryAsync(CategoryName, 1, int.MaxValue)
                                                 .ToListAsync();
            Assert.AreEqual(1, species.Count());
            Assert.AreEqual(SpeciesName, species.First().Name);
            Assert.AreEqual(CategoryName, species.First().Category.Name);
        }

        [TestMethod]
        public void ListByMissingCategoryTest()
        {
            IEnumerable<Species> species = _factory.Species.ListByCategory("Missing", 1, int.MaxValue);
            Assert.IsFalse(species.Any());
        }

        [TestMethod]
        public void RenameTest()
        {
            Species species = _factory.Species.Rename(SpeciesName, RenameSpeciesName);
            Assert.AreEqual(species.Name, RenameSpeciesName);

            Species original = _factory.Species.Get(s => s.Name == SpeciesName);
            Assert.IsNull(original);
        }

        [TestMethod]
        public async Task RenameAsyncTest()
        {
            Species species = await _factory.Species.RenameAsync(SpeciesName, RenameSpeciesName);
            Assert.AreEqual(species.Name, RenameSpeciesName);

            Species original = await _factory.Species.GetAsync(s => s.Name == SpeciesName);
            Assert.IsNull(original);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void RenameMissingTest()
        {
            _factory.Species.Rename("", RenameSpeciesName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public async Task RenameMissingAsyncTest()
        {
            await _factory.Species.RenameAsync("", RenameSpeciesName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesAlreadyExistsException))]
        public void RenameToExistingTest()
        {
            _factory.Species.Rename(SpeciesName, SpeciesName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesAlreadyExistsException))]
        public async Task RenameToExistingAsyncTest()
        {
            await _factory.Species.RenameAsync(SpeciesName, SpeciesName);
        }

        [TestMethod]
        public void MoveSpeciesTest()
        {
            _factory.Categories.Add(MoveCategoryName);
            Species species = _factory.Species.Move(SpeciesName, MoveCategoryName);
            Assert.AreEqual(MoveCategoryName, species.Category.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesIsAlreadyInCategoryException))]
        public void MoveToCurrentCategoryTest()
        {
            _factory.Species.Move(SpeciesName, CategoryName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void MoveMissingSpeciesTest()
        {
            _factory.Species.Move("", MoveCategoryName);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void MoveToMissingCategoryTest()
        {
            _factory.Species.Move(SpeciesName, "");
        }

        [TestMethod]
        public async Task MoveSpeciesAsyncTest()
        {
            await _factory.Categories.AddAsync(MoveCategoryName);
            Species species = await _factory.Species.MoveAsync(SpeciesName, MoveCategoryName);
            Assert.AreEqual(MoveCategoryName, species.Category.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesIsAlreadyInCategoryException))]
        public async Task MoveToCurrentCategoryAsyncTest()
        {
            await _factory.Species.MoveAsync(SpeciesName, CategoryName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public async Task MoveMissingSpeciesAsyncTest()
        {
            await _factory.Species.MoveAsync("", MoveCategoryName);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public async Task MoveToMissingCategoryAsyncTest()
        {
            await _factory.Species.MoveAsync(SpeciesName, "");
        }
    }
}
