using System.IO;
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
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ListCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "something" }
                    });
                }
            }
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

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ListCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "locations" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "list-locations.txt");
        }

        [TestMethod]
        public void ListCategoriesCommandTest()
        {
            _factory.Categories.Add("birds");

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ListCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "categories" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "list-categories.txt");
        }

        [TestMethod]
        public void ListSpeciesCommandTest()
        {
            _factory.Categories.Add("birds");
            _factory.Species.Add("blackbird", "birds");

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ListCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "species", "birds" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "list-species.txt");
        }

        [TestMethod]
        public void ListUsersCommandTest()
        {
            _factory.Users.AddUser("someone", "somepassword");

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ListCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "users" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "list-users.txt");
        }
    }
}
