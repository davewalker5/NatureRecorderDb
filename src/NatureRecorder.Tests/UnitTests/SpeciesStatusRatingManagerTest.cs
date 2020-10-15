using System;
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
    public class SpeciesStatusRatingTest
    {
        private const string CategoryName = "Birds";
        private const string SpeciesName = "Nightingale";
        private const string SchemeName = "BOCC4";
        private const string RatingName = "Red";
        private const string NewRatingName = "Amber";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _factory.Categories.Add(CategoryName);
            _factory.Species.Add(SpeciesName, CategoryName);

            _factory.StatusSchemes.Add(SchemeName);
            _factory.StatusRatings.Add(RatingName, SchemeName);

            _factory.SpeciesStatusRatings.SetRating(SpeciesName, RatingName, SchemeName);
        }

        [TestMethod]
        public void GetCurrentRatingTest()
        {
            SpeciesStatusRating entity = _factory.SpeciesStatusRatings.GetCurrent(SpeciesName, SchemeName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(SpeciesName, entity.Species.Name);
            Assert.AreEqual(RatingName, entity.Rating.Name);
            Assert.AreEqual(SchemeName, entity.Rating.Scheme.Name);

            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            Assert.AreEqual(startDate, entity.Start);
            Assert.IsNull(entity.End);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void SetRatingForMissingSpeciesTest()
        {
            _factory.SpeciesStatusRatings.SetRating("", RatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public async Task SetRatingForMissingSpeciesAsyncTest()
        {
            await _factory.SpeciesStatusRatings.SetRatingAsync("", RatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void SetInvalidRatingTest()
        {
            _factory.SpeciesStatusRatings.SetRating(SpeciesName, "", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public async Task SetInvalidRatingAsyncTest()
        {
            await _factory.SpeciesStatusRatings.SetRatingAsync(SpeciesName, "", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void SetInvalidSchemeTest()
        {
            _factory.SpeciesStatusRatings.SetRating(SpeciesName, RatingName, "");
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public async Task SetInvalidSchemeAsyncTest()
        {
            await _factory.SpeciesStatusRatings.SetRatingAsync(SpeciesName, RatingName, "");
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void GetCurrentForMissingSpeciesTest()
        {
            _factory.SpeciesStatusRatings.GetCurrent("", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public void GetCurrentForMissingSchemeTest()
        {
            _factory.SpeciesStatusRatings.GetCurrent(SpeciesName, "");
        }

        [TestMethod]
        public async Task GetCurrentRatingAsyncTest()
        {
            SpeciesStatusRating entity = await _factory.SpeciesStatusRatings.GetCurrentAsync(SpeciesName, SchemeName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(SpeciesName, entity.Species.Name);
            Assert.AreEqual(RatingName, entity.Rating.Name);
            Assert.AreEqual(SchemeName, entity.Rating.Scheme.Name);

            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            Assert.AreEqual(startDate, entity.Start);
            Assert.IsNull(entity.End);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public async Task GetCurrentForMissingSpeciesAsyncTest()
        {
            await _factory.SpeciesStatusRatings.GetCurrentAsync("", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusSchemeDoesNotExistException))]
        public async Task GetCurrentForMissingSchemeAsyncTest()
        {
            await _factory.SpeciesStatusRatings.GetCurrentAsync(SpeciesName, "");
        }

        [TestMethod]
        public void ReplaceExistingRatingTest()
        {
            // Calculate the new start date and, from that, both the start and end dates for
            // the record that's about to be replaced
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime replacedDates = startDate.AddSeconds(-1);

            _factory.StatusRatings.Add(NewRatingName, SchemeName);
            _factory.SpeciesStatusRatings.SetRating(SpeciesName, NewRatingName, SchemeName);
            IEnumerable<SpeciesStatusRating> ratings = _factory.SpeciesStatusRatings
                                                               .List(null, 1, int.MaxValue)
                                                               .OrderBy(r => r.Start);

            Assert.AreEqual(2, ratings.Count());
            Assert.AreEqual(RatingName, ratings.First().Rating.Name);
            Assert.AreEqual(replacedDates, ratings.First().Start);
            Assert.AreEqual(replacedDates, ratings.First().End);

            Assert.AreEqual(NewRatingName, ratings.Last().Rating.Name);
            Assert.AreEqual(startDate, ratings.Last().Start);
            Assert.IsNull(ratings.Last().End);
        }

        [TestMethod]
        public async Task ReplaceExistingRatingAsyncTest()
        {
            // Calculate the new start date and, from that, both the start and end dates for
            // the record that's about to be replaced
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime replacedDates = startDate.AddSeconds(-1);

            await _factory.StatusRatings.AddAsync(NewRatingName, SchemeName);
            await _factory.SpeciesStatusRatings.SetRatingAsync(SpeciesName, NewRatingName, SchemeName);
            List<SpeciesStatusRating> ratings = await _factory.SpeciesStatusRatings
                                                              .ListAsync(null, 1, int.MaxValue)
                                                              .OrderBy(r => r.Start)
                                                              .ToListAsync();

            Assert.AreEqual(2, ratings.Count());
            Assert.AreEqual(RatingName, ratings.First().Rating.Name);
            Assert.AreEqual(replacedDates, ratings.First().Start);
            Assert.AreEqual(replacedDates, ratings.First().End);

            Assert.AreEqual(NewRatingName, ratings.Last().Rating.Name);
            Assert.AreEqual(startDate, ratings.Last().Start);
            Assert.IsNull(ratings.Last().End);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void AddToMissingSpeciesTest()
        {
            _factory.SpeciesStatusRatings.SetRating("", RatingName, SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void AddMissingRatingTest()
        {
            _factory.SpeciesStatusRatings.SetRating(SpeciesName, "", SchemeName);
        }

        [TestMethod]
        [ExpectedException(typeof(StatusRatingDoesNotExistException))]
        public void AddRatingForMissingSchemeTest()
        {
            _factory.SpeciesStatusRatings.SetRating(SpeciesName, RatingName, "");
        }

        [TestMethod]
        public void ClearRatingTest()
        {
            _factory.SpeciesStatusRatings.ClearRating(SpeciesName, SchemeName);
            SpeciesStatusRating rating = _factory.SpeciesStatusRatings.GetCurrent(SpeciesName, SchemeName);
            Assert.IsNull(rating);
        }

        [TestMethod]
        public async Task ClearRatingAsycnTest()
        {
            await _factory.SpeciesStatusRatings.ClearRatingAsync(SpeciesName, SchemeName);
            SpeciesStatusRating rating = _factory.SpeciesStatusRatings.GetCurrent(SpeciesName, SchemeName);
            Assert.IsNull(rating);
        }
    }
}
