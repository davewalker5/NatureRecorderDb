using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class DataViewingCommandTest
    {
        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void ListInvalidEntityTypeCommandTest()
        {
            string[] arguments = new string[] { "something" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, null, 0);
        }

        [TestMethod]
        public void ListLocationsCommandTest()
        {
            _factory.Locations.Add("Radley Lakes",
                                   null,
                                   "Abingdon",
                                   "Oxfordshire",
                                   "OX14 3NE",
                                   "United Kingdom",
                                   51.6741263M,
                                   -1.2496157M);

            string[] arguments = new string[] { "locations" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-locations.txt", 0);
        }

        [TestMethod]
        public void ListCategoriesCommandTest()
        {
            _factory.Categories.Add("birds");

            string[] arguments = new string[] { "categories" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-categories.txt", 0);
        }

        [TestMethod]
        public void ListSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("blackbird", "birds");

            string[] arguments = new string[] { "species", "birds" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-species.txt", 0);
        }

        [TestMethod]
        public void ListUsersCommandTest()
        {
            _factory.Users.AddUser("someone", "somepassword");

            string[] arguments = new string[] { "users" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-users.txt", 0);
        }

        [TestMethod]
        public void ListSchemesTest()
        {
            _factory.StatusSchemes.Add("BOCC4");

            string[] arguments = new string[] { "schemes" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-schemes.txt", 0);
        }

        [TestMethod]
        public void ListRatingsTest()
        {
            _factory.StatusSchemes.Add("BOCC4");
            _factory.StatusRatings.Add("Red", "BOCC4");
            _factory.StatusRatings.Add("Amber", "BOCC4");

            string[] arguments = new string[] { "ratings", "BOCC4" };
            TestHelpers.RunCommand(_factory, arguments, new ListCommand(), CommandMode.CommandLine, null, null, null, "list-ratings.txt", 0);
        }
    }
}
