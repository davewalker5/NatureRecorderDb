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
    public class StatusRatingManagerTest
    {
        private const string SchemeName = "BOCC4";
        private const string RatingName = "Red";
        private const string RenamedRatingName = "Green";
        private const string AsyncRatingName = "Amber";

        private NatureRecorderFactory _factory;
        private int _schemeId;
        private int _ratingId;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _schemeId = _factory.StatusSchemes.Add(SchemeName).Id;
            _ratingId = _factory.StatusRatings.Add(RatingName, SchemeName).Id;
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.StatusRatings.Add(RatingName, SchemeName);
            Assert.AreEqual(1, _factory.StatusRatings.List(null, 1, int.MaxValue).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            StatusRating entity = _factory.StatusRatings.Get(a => (a.Name == RatingName) && (a.Scheme.Name == SchemeName));
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(RatingName, entity.Name);
            Assert.AreEqual(SchemeName, entity.Scheme.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.StatusRatings.AddAsync(AsyncRatingName, SchemeName);
            StatusRating entity = await _factory.StatusRatings.GetAsync(a => (a.Name == AsyncRatingName) && (a.Scheme.Name == SchemeName));
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(AsyncRatingName, entity.Name);
            Assert.AreEqual(SchemeName, entity.Scheme.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void AddToInvalidSchemeTest()
        {
            _factory.StatusRatings.Add("Blue", "");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task AddToInvalidSchemeAsyncTest()
        {
            await _factory.StatusRatings.AddAsync("Blue", "");
        }

        [TestMethod]
        public void GetMissingTest()
        {
            StatusRating entity = _factory.StatusRatings.Get(a => (a.Name == "Missing") && (a.Scheme.Name == SchemeName));
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<StatusRating> entities = _factory.StatusRatings.List(null, 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(RatingName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<StatusRating> entities = await _factory.StatusRatings
                                                        .ListAsync(null, 1, int.MaxValue)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(RatingName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<StatusRating> entities = _factory.StatusRatings
                                                         .List(a => (a.Name == RatingName) && (a.Scheme.Name == SchemeName), 1, int.MaxValue);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(RatingName, entities.First().Name);
            Assert.AreEqual(SchemeName, entities.First().Scheme.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<StatusRating> entities = await _factory.StatusRatings
                                                        .ListAsync(a => (a.Name == RatingName) && (a.Scheme.Name == SchemeName), 1, int.MaxValue)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(RatingName, entities.First().Name);
            Assert.AreEqual(SchemeName, entities.First().Scheme.Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<StatusRating> entities = _factory.StatusRatings
                                                         .List(a => (a.Name == "Missing") && (a.Scheme.Name == SchemeName), 1, int.MaxValue);
            Assert.AreEqual(0, entities.Count());
        }

        [TestMethod]
        public void RenameTest()
        {
            StatusRating rating = _factory.StatusRatings
                                          .Rename(RatingName, RenamedRatingName, SchemeName);
            Assert.AreEqual(rating.Name, RenamedRatingName);

            StatusRating original = _factory.StatusRatings.Get(s => s.Name == RatingName);
            Assert.IsNull(original);
        }

        [TestMethod]
        public async Task RenameAsyncTest()
        {
            StatusRating rating = await _factory.StatusRatings
                                                 .RenameAsync(RatingName, RenamedRatingName, SchemeName);
            Assert.AreEqual(rating.Name, RenamedRatingName);

            StatusRating original = await _factory.StatusRatings
                                                  .GetAsync(s => s.Name == RatingName);
            Assert.IsNull(original);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void RenameMissingTest()
        {
            _factory.StatusRatings.Rename("", RenamedRatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public async Task RenameMissingAsyncTest()
        {
            await _factory.StatusRatings.RenameAsync("", RenamedRatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingAlreadyExistsException))]
        public void RenameToExistingTest()
        {
            _factory.StatusRatings.Add(RenamedRatingName, SchemeName);
            _factory.StatusRatings.Rename(RatingName, RenamedRatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingAlreadyExistsException))]
        public async Task RenameToExistingAsyncTest()
        {
            await _factory.StatusRatings.AddAsync(RenamedRatingName, SchemeName);
            await _factory.StatusRatings.RenameAsync(RatingName, RenamedRatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void RenameForInvalidSchemeTest()
        {
            _factory.StatusRatings.Rename(RatingName, RenamedRatingName, "");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task RenameForInvalidSchemeAsyncTest()
        {
            await _factory.StatusRatings.RenameAsync(RatingName, RenamedRatingName, "");
        }

        [TestMethod]
        public void DeleteTest()
        {
            _factory.StatusRatings.Delete(RatingName, SchemeName);
            IEnumerable<StatusRating> ratings = _factory.StatusRatings.List(null, 1, int.MaxValue);
            Assert.IsFalse(ratings.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void DeleteMissingTest()
        {
            _factory.StatusRatings.Delete("", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingIsInUseException))]
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
            _factory.StatusRatings.Delete(RatingName, SchemeName);
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            await _factory.StatusRatings.DeleteAsync(RatingName, SchemeName);
            IEnumerable<StatusRating> entities = await _factory.StatusRatings.ListAsync(null, 1, int.MaxValue).ToListAsync();
            Assert.IsFalse(entities.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public async Task DeleteMissingAsyncTest()
        {
            await _factory.StatusRatings.DeleteAsync("", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingIsInUseException))]
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
            await _factory.StatusRatings.DeleteAsync(RatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void DeleteFromInvalidSchemeTest()
        {
            _factory.StatusRatings.Delete(RatingName, "");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task DeleteFromInvalidSchemeAsyncTest()
        {
            await _factory.StatusRatings.DeleteAsync(RatingName, "");
        }
    }
}
