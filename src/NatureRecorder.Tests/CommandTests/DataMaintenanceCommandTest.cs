using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class DataMaintenanceCommandTest
    {
        private NatureRecorderFactory _factory;
        private string _currentFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DataMaintenanceCommandTest)).Location);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void AddInvalidEntityCommandTest()
        {
            string[] arguments = new string[] { "something" };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void AddLocationCommandTest()
        {
            Assert.IsFalse(_factory.Locations.List(null, 1, int.MaxValue).Any());

            string[] arguments = new string[] { "location" };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.Interactive, null, null, "add-location.txt", null, 0);

            Location location = _factory.Locations
                                        .List(null, 1, int.MaxValue)
                                        .FirstOrDefault();
            Assert.IsNotNull(location);
            Assert.AreEqual("Radley Lakes", location.Name);
            Assert.IsTrue(string.IsNullOrEmpty(location.Address));
            Assert.AreEqual("Abingdon", location.City);
            Assert.AreEqual("Oxfordshire", location.County);
            Assert.AreEqual("OX14 3NE", location.Postcode);
            Assert.AreEqual("United Kingdom", location.Country);
            Assert.AreEqual(51.6741263M, location.Latitude);
            Assert.AreEqual(-1.2496157M, location.Longitude);
        }

        [TestMethod]
        public void AddCategoryCommandTest()
        {
            Assert.IsFalse(_factory.Categories.List(null, 1, int.MaxValue).Any());

            string[] arguments = new string[] { "category" };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.Interactive, null, null, "add-category.txt", null, 0);

            Category category = _factory.Categories
                                        .List(null, 1, int.MaxValue)
                                        .FirstOrDefault();
            Assert.IsNotNull(category);
            Assert.AreEqual("Birds", category.Name);
        }

        [TestMethod]
        public void AddSpeciesCommandTest()
        {
            Assert.IsFalse(_factory.Species.List(null, 1, int.MaxValue).Any());

            _factory.Categories.Add("birds");

            string[] arguments = new string[] { "species" };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.Interactive, null, null, "add-species.txt", null, 0);

            Species species = _factory.Species
                                      .List(null, 1, int.MaxValue)
                                      .FirstOrDefault();
            Assert.IsNotNull(species);
            Assert.AreEqual("Canada Goose", species.Name);
            Assert.AreEqual("Birds", species.Category.Name);
        }

        [TestMethod]
        public void AddSightingCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");
            _factory.Locations.Add("Radley Lakes", null, null, null, null, null, null, null);

            string[] arguments = new string[] { "sighting" };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.Interactive, null, "naturerecorder.settings", "add-sighting.txt", null, 0);

            Sighting sighting = _factory.Sightings
                                        .List(null, 1, int.MaxValue)
                                        .FirstOrDefault();
            Assert.IsNotNull(sighting);
            Assert.AreEqual("Robin", sighting.Species.Name);
            Assert.AreEqual("Birds", sighting.Species.Category.Name);
            Assert.AreEqual("Radley Lakes", sighting.Location.Name);
            Assert.AreEqual(new DateTime(2020, 1, 1), sighting.Date);
            Assert.AreEqual(0, sighting.Number);
            Assert.AreEqual(Gender.Unknown, sighting.Gender);
            Assert.IsFalse(sighting.WithYoung);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void RenameInvalidEntityCommandTest()
        {
            string[] arguments = new string[] { "something", "value", "value" };
            TestHelpers.RunCommand(_factory, arguments, new RenameCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void RenameCategoryCommandTest()
        {
            _factory.Categories.Add("birds");

            string[] arguments = new string[] { "category", "birds", "things with wings" };
            TestHelpers.RunCommand(_factory, arguments, new RenameCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Category category = _factory.Categories.Get(s => s.Name == "Things With Wings");
            Assert.IsNotNull(category);
        }

        [TestMethod]
        public void RenameSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("blackbird", "birds");

            string[] arguments = new string[] { "species", "blackbird", "robin" };
            TestHelpers.RunCommand(_factory, arguments, new RenameCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Species species = _factory.Species.Get(s => s.Name == "Robin");
            Assert.IsNotNull(species);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void MoveInvalidEntityCommandTest()
        {
            string[] arguments = new string[] { "something", "value", "value" };
            TestHelpers.RunCommand(_factory, arguments, new MoveCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void MoveSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Categories.Add("things with wings");
            _factory.Species.Add("blackbird", "birds");

            string[] arguments = new string[] { "species", "blackbird", "things with wings" };
            TestHelpers.RunCommand(_factory, arguments, new MoveCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Species species = _factory.Species.Get(s => s.Name == "Blackbird");
            Assert.AreEqual(species.Category.Name, "Things With Wings");
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void DeleteInvalidEntityCommandTest()
        {
            string[] arguments = new string[] { "something", "id" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void DeleteLocationCommandTest()
        {
            _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);

            string[] arguments = new string[] { "location", "somewhere" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Location location = _factory.Locations.Get(l => l.Name == "somewhere");
            Assert.IsNull(location);
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void DeleteMissingLocationCommandTest()
        {
            string[] arguments = new string[] { "location", "somewhere" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(LocationIsInUseException))]
        public void DeleteInUseLocationCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            _factory.Sightings.Add(0, Gender.Unknown, false, DateTime.Now, location.Id, species.Id);

            string[] arguments = new string[] { "location", "somewhere" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void DeleteCategoryCommandTest()
        {
            _factory.Categories.Add("birds");

            string[] arguments = new string[] { "category", "birds" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Category category = _factory.Categories.Get(c => c.Name == "birds");
            Assert.IsNull(category);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void DeleteMissingCategoryCommandTest()
        {
            string[] arguments = new string[] { "category", "birds" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryIsInUseException))]
        public void DeleteInUseCategoryCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");

            string[] arguments = new string[] { "category", "birds" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void DeleteSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");

            string[] arguments = new string[] { "species", "robin" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Species species = _factory.Species.Get(l => l.Name == "robin");
            Assert.IsNull(species);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void DeleteMissingSpeciesCommandTest()
        {
            string[] arguments = new string[] { "species", "robin" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesIsInUseException))]
        public void DeleteInUseSpeciesCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            _factory.Sightings.Add(0, Gender.Unknown, false, DateTime.Now, location.Id, species.Id);

            string[] arguments = new string[] { "species", "robin" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void DeleteSightingCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            Sighting sighting = _factory.Sightings.Add(0, Gender.Unknown, false, DateTime.Now, location.Id, species.Id);

            string[] arguments = new string[] { "sighting", sighting.Id.ToString() };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);

            Assert.IsFalse(_factory.Context.Sightings.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(SightingDoesNotExistException))]
        public void DeleteMissingSightingCommandTest()
        {
            string[] arguments = new string[] { "sighting", "1" };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.Interactive, null, null, null, null, 0);
        }

        [TestMethod]
        public void EditLocationCommandTest()
        {
            _factory.Locations.Add("Radley Lakes", null, null, null, null, null, null, null);

            Location location = _factory.Locations
                                        .List(null, 1, int.MaxValue)
                                        .FirstOrDefault();
            Assert.AreEqual("Radley Lakes", location.Name);
            Assert.IsNull(location.Address);
            Assert.IsNull(location.City);
            Assert.IsNull(location.County);
            Assert.IsNull(location.Postcode);
            Assert.IsNull(location.Country);
            Assert.IsNull(location.Latitude);
            Assert.IsNull(location.Longitude);

            // The prompting during editing is identical to that during adding new locations
            // so we can reuse the "add" test input file
            string[] arguments = new string[] { "location", "Radley Lakes" };
            TestHelpers.RunCommand(_factory, arguments, new EditCommand(), CommandMode.Interactive, null, null, "add-location.txt", null, 0);

            location = _factory.Locations
                               .List(null, 1, int.MaxValue)
                               .FirstOrDefault();
            Assert.AreEqual("Radley Lakes", location.Name);
            Assert.IsTrue(string.IsNullOrEmpty(location.Address));
            Assert.AreEqual("Abingdon", location.City);
            Assert.AreEqual("Oxfordshire", location.County);
            Assert.AreEqual("OX14 3NE", location.Postcode);
            Assert.AreEqual("United Kingdom", location.Country);
            Assert.AreEqual(51.6741263M, location.Latitude);
            Assert.AreEqual(-1.2496157M, location.Longitude);
        }
    }
}
