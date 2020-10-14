using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;
using NatureRecorder.Tests.Wrappers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class CommandBaseTests
    {
        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
        }

        [TestMethod]
        public void InvalidCommandModeTest()
        {
            string[] arguments = new string[] { "category" };
            string data = TestHelpers.RunCommand(null, arguments, new AddCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("is not valid for command mode"));
        }

        [TestMethod]
        public void TooFewArgumentsTest()
        {
            string data = TestHelpers.RunCommand(null, null, new ImportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("expects"));
        }

        [TestMethod]
        public void TooManyArgumentsTest()
        {
            string[] arguments = new string[] { "importtype", "filename", "extra" };
            string data = TestHelpers.RunCommand(null, arguments, new ImportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("expects"));
        }

        [TestMethod]
        public void TestGetWildcardLocationId()
        {
            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Location, "*");
            Assert.IsNull(id);
        }

        [TestMethod]
        public void TestGetLocationId()
        {
            _factory.Locations.Add("somewhere", null, null, null, null, null, null, null);

            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Location, "somewhere");
            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void TestGetMissingLocationId()
        {
            new CommandBaseWrapper(_factory).GetEntityId(EntityType.Location, "somewhere");
        }

        [TestMethod]
        public void TestGetWildcardCategoryId()
        {
            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Category, "*");
            Assert.IsNull(id);
        }

        [TestMethod]
        public void TestGetCategoryId()
        {
            _factory.Categories.Add("birds");

            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Category, "birds");
            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void TestGetMissingCategoryId()
        {
            new CommandBaseWrapper(_factory).GetEntityId(EntityType.Category, "birds");
        }

        [TestMethod]
        public void TestGetWildcardSpeciesId()
        {
            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Species, "*");
            Assert.IsNull(id);
        }

        [TestMethod]
        public void TestGetSpeciesId()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");

            int? id = new CommandBaseWrapper(_factory).GetEntityId(EntityType.Species, "robin");
            Assert.IsNotNull(id);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void TestGetMissingSpeciesId()
        {
            new CommandBaseWrapper(_factory).GetEntityId(EntityType.Species, "robin");
        }

        [TestMethod]
        public void TestGetDate()
        {
            DateTime? date = new CommandBaseWrapper(_factory).GetDate("2020-01-01");
            Assert.AreEqual(new DateTime(2020, 1, 1), date);
        }

        [TestMethod]
        public void TestGetInvalidDate()
        {
            DateTime? date = new CommandBaseWrapper(_factory).GetDate("gibberish");
            Assert.IsNull(date);
        }
    }
}
