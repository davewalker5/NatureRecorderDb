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
using NatureRecorder.Interpreter.Logic;
using NatureRecorder.Tests.UnitTests;

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

            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void AddInvalidEntityCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new AddCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "something" }
                    });
                }
            }
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
                    new AddCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location" }
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
                    new AddCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "category" }
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
                    new AddCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species" }
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

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void RenameInvalidEntityCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new RenameCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "something", "value", "value" }
                    });
                }
            }
        }

        [TestMethod]
        public void RenameCategoryCommandTest()
        {
            _factory.Categories.Add("birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new RenameCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "category", "birds", "things with wings" }
                    });
                }
            }

            Category category = _factory.Categories.Get(s => s.Name == "Things With Wings");
            Assert.IsNotNull(category);
        }

        [TestMethod]
        public void RenameSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("blackbird", "birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new RenameCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species", "blackbird", "robin" }
                    });
                }
            }

            Species species = _factory.Species.Get(s => s.Name == "Robin");
            Assert.IsNotNull(species);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void MoveInvalidEntityCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new MoveCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "something", "value", "value" }
                    });
                }
            }
        }

        [TestMethod]
        public void MoveSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Categories.Add("things with wings");
            _factory.Species.Add("blackbird", "birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new MoveCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species", "blackbird", "things with wings" }
                    });
                }
            }

            Species species = _factory.Species.Get(s => s.Name == "Blackbird");
            Assert.AreEqual(species.Category.Name, "Things With Wings");
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEntityTypeException))]
        public void DeleteInvalidEntityCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "something", "id" }
                    });
                }
            }
        }

        [TestMethod]
        public void DeleteLocationCommandTest()
        {
            _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", "somewhere" }
                    });
                }
            }

            Location location = _factory.Locations.Get(l => l.Name == "somewhere");
            Assert.IsNull(location);
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void DeleteMissingLocationCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", "somewhere" }
                    });
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(LocationIsInUseException))]
        public void DeleteInUseLocationCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            _factory.Sightings.Add(0, false, DateTime.Now, location.Id, species.Id);

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", "somewhere" }
                    });
                }
            }
        }

        [TestMethod]
        public void DeleteCategoryCommandTest()
        {
            _factory.Categories.Add("birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "category", "birds" }
                    });
                }
            }

            Location location = _factory.Locations.Get(l => l.Name == "somewhere");
            Assert.IsNull(location);
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryDoesNotExistException))]
        public void DeleteMissingCategoryCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "category", "birds" }
                    });
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CategoryIsInUseException))]
        public void DeleteInUseCategoryCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "category", "birds" }
                    });
                }
            }
        }

        [TestMethod]
        public void DeleteSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("robin", "birds");

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species", "robin" }
                    });
                }
            }

            Species species = _factory.Species.Get(l => l.Name == "robin");
            Assert.IsNull(species);
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesDoesNotExistException))]
        public void DeleteMissingSpeciesCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species", "robin" }
                    });
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpeciesIsInUseException))]
        public void DeleteInUseSpeciesCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            _factory.Sightings.Add(0, false, DateTime.Now, location.Id, species.Id);

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "species", "robin" }
                    });
                }
            }
        }

        [TestMethod]
        public void DeleteSightingCommandTest()
        {
            Location location = _factory.Locations.Add("somewhere", "", "", "", "", "", null, null);
            _factory.Categories.Add("birds");
            Species species = _factory.Species.Add("robin", "birds");
            Sighting sighting = _factory.Sightings.Add(0, false, DateTime.Now, location.Id, species.Id);

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "sighting", sighting.Id.ToString() }
                    });
                }
            }

            Assert.IsFalse(_factory.Context.Sightings.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(SightingDoesNotExistException))]
        public void DeleteMissingSightingCommandTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new DeleteCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "sighting", "1" }
                    });
                }
            }
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

            // The promting during editing is identical to that during adding new locations
            // so we can reuse the "add" test input file
            string commandFilePath = Path.Combine(_currentFolder, "Content", "add-location.txt");
            using (StreamReader input = new StreamReader(commandFilePath))
            {
                using (StreamWriter output = new StreamWriter(new MemoryStream()))
                {
                    new EditCommand().Run(new CommandContext
                    {
                        Reader = new StreamCommandReader(input),
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", "Radley Lakes" }
                    });
                }
            }

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
