using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Interpreter.Logic;

namespace NatureRecorder.Tests
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

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);
        }

        [TestMethod]
        public void AddLocationCommandTest()
        {
            Assert.IsFalse(_factory.Locations.List(null, 1, int.MaxValue).Any());

            string commandFilePath = Path.Combine(_currentFolder, "Content", "add-location.txt");
            using (StreamReader input = new StreamReader(commandFilePath))
            {
                using (StreamWriter output = new StreamWriter(new MemoryStream()))
                {
                    new AddLocationCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { }
                    });
                }
            }

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

            string commandFilePath = Path.Combine(_currentFolder, "Content", "add-category.txt");
            using (StreamReader input = new StreamReader(commandFilePath))
            {
                using (StreamWriter output = new StreamWriter(new MemoryStream()))
                {
                    new AddCategoryCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { }
                    });
                }
            }

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

            string commandFilePath = Path.Combine(_currentFolder, "Content", "add-species.txt");
            using (StreamReader input = new StreamReader(commandFilePath))
            {
                using (StreamWriter output = new StreamWriter(new MemoryStream()))
                {
                    new AddSpeciesCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { }
                    });
                }
            }

            Species species = _factory.Species
                                      .List(null, 1, int.MaxValue)
                                      .FirstOrDefault();
            Assert.IsNotNull(species);
            Assert.AreEqual("Canada Goose", species.Name);
            Assert.AreEqual("Birds", species.Category.Name);
        }
    }
}
