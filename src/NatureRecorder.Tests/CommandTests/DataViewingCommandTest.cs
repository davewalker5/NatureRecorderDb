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
    }
}
