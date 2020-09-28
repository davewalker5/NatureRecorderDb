using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;
using NatureRecorder.Tests.UnitTests;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class ReportingCommandTest
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
        [ExpectedException(typeof(UnknownReportTypeException))]
        public void InvalidReportTypeTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ReportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "invalid" }
                    });
                }
            }
        }

        [TestMethod]
        public void SummaryReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ReportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "summary", "1996-11-23", "2000-01-01" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "report-summary.txt", 0);
        }

        [TestMethod]
        public void LocationReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ReportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "location", "bagley wood", "1996-11-23", "2000-01-31" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "report-location.txt", 0);
        }

        [TestMethod]
        public void CategoryReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ReportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "category", "birds", "bagley wood", "1996-11-23", "2000-01-01" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "report-category.txt", 0);
        }

        [TestMethod]
        public void SpeciesReportTest()
        {
            string importFilePath = Path.Combine(_currentFolder, "Content", "valid-import.csv");
            _factory.Import.Import(importFilePath);

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ReportCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "species", "jackdaw", "bagley wood", "1996-11-23", "2000-01-01" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "report-category.txt", 0);
        }
    }
}
