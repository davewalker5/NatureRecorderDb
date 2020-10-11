using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class StatusSchemeManagerTest
    {
        private const string EntityName = "BOCC4";
        private const string AsyncEntityName = "Birds of Consevation Concern 4";
        private const string RenamedEntityName = "A Status Scheme";

        private NatureRecorderFactory _factory;
        private int _schemeId;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _schemeId = _factory.StatusSchemes.Add(EntityName).Id;
            _factory.Context.StatusRatings.Add(new StatusRating { Name = "Red", StatusSchemeId = _schemeId });
            _factory.Context.StatusRatings.Add(new StatusRating { Name = "Amber", StatusSchemeId = _schemeId });
            _factory.Context.StatusRatings.Add(new StatusRating { Name = "Green", StatusSchemeId = _schemeId });
            _factory.Context.SaveChanges();
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
            StatusScheme entity = _factory.StatusSchemes.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.StatusSchemes.AddAsync(AsyncEntityName);
            StatusScheme entity = await _factory.StatusSchemes.GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            StatusScheme entity = _factory.StatusSchemes.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<StatusScheme> entities = _factory.StatusSchemes.List(null, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<StatusScheme> entities = await _factory.StatusSchemes
                                                        .ListAsync(null, 1, int.MaxValue)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<StatusScheme> entities = _factory.StatusSchemes
                                                         .List(e => e.Name == EntityName, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<StatusScheme> entities = await _factory.StatusSchemes
                                                        .ListAsync(e => e.Name == EntityName, 1, int.MaxValue)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<StatusScheme> entities = _factory.StatusSchemes
                                                         .List(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, entities.Count());
        }

        [TestMethod]
        public void RenameTest()
        {
            StatusScheme scheme = _factory.StatusSchemes
                                            .Rename(EntityName, RenamedEntityName);
            Assert.AreEqual(scheme.Name, RenamedEntityName);

            StatusScheme original = _factory.StatusSchemes.Get(s => s.Name == EntityName);
            Assert.IsNull(original);
        }

        [TestMethod]
        public async Task RenameAsyncTest()
        {
            StatusScheme scheme = await _factory.StatusSchemes
                                                  .RenameAsync(EntityName, RenamedEntityName);
            Assert.AreEqual(scheme.Name, RenamedEntityName);

            StatusScheme original = await _factory.StatusSchemes
                                                  .GetAsync(s => s.Name == EntityName);
            Assert.IsNull(original);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void RenameMissingTest()
        {
            _factory.StatusSchemes.Rename("", RenamedEntityName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task RenameMissingAsyncTest()
        {
            await _factory.StatusSchemes.RenameAsync("", RenamedEntityName);
        }

        [TestMethod]
        public void DeleteTest()
        {
            _factory.StatusSchemes.Delete(EntityName);
            IEnumerable<StatusScheme> schemes = _factory.StatusSchemes.List(null, 1, int.MaxValue);
            Assert.IsFalse(schemes.Any());
            Assert.IsFalse(_factory.Context.StatusRatings.Any(r => r.StatusSchemeId == _schemeId));
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void DeleteMissingTest()
        {
            _factory.StatusSchemes.Delete("");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeIsInUseException))]
        public void DeleteInUseTest()
        {
            _factory.Categories.Add("Birds");
            Species species = _factory.Species.Add("Robin", "Birds");
            _factory.Context.SpeciesStatusRatings.Add(new SpeciesStatusRating
            {
                SpeciesId = species.Id,
                StatusRatingId = _factory.Context.StatusRatings.First().Id
            });

            _factory.Context.SaveChanges();
            _factory.StatusSchemes.Delete(EntityName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            await _factory.StatusSchemes.DeleteAsync(EntityName);
            IEnumerable<StatusScheme> entities = await _factory.StatusSchemes.ListAsync(null, 1, int.MaxValue).ToListAsync();
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task DeleteMissingAsyncTest()
        {
            await _factory.StatusSchemes.DeleteAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeIsInUseException))]
        public async Task DeleteInUseAsyncTest()
        {
            _factory.Categories.Add("Birds");
            Species species = _factory.Species.Add("Robin", "Birds");
            _factory.Context.SpeciesStatusRatings.Add(new SpeciesStatusRating
            {
                SpeciesId = species.Id,
                StatusRatingId = _factory.Context.StatusRatings.First().Id
            });

            await _factory.Context.SaveChangesAsync();
            await _factory.StatusSchemes.DeleteAsync(EntityName);
        }
    }
}
