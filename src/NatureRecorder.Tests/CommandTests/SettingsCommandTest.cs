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
	public class SettingsCommandTest
    {
        private const string Location = "My Default Location";

        private UserSettings _settings = new UserSettings();
        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
            _settings = new UserSettings(Path.GetTempFileName());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(_settings.SettingsFilePath))
            {
                File.Delete(_settings.SettingsFilePath);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void SetInvalidLocationTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new SettingsCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", Location },
                        Settings = _settings
                    });
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownSettingTypeException))]
        public void RequestInvalidActionTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new SettingsCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "notvalid" },
                        Settings = _settings
                    });
                }
            }
        }

        [TestMethod]
        public void SetLocationTest()
        {
            _factory.Locations.Add(Location, "", "", "", "", "", null, null);

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new SettingsCommand().Run(new CommandContext
                    {
                        Output = output,
                        Factory = _factory,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "location", Location },
                        Settings = _settings
                    });
                }
            }

            Assert.AreEqual(Location, _settings.Location);
            TestHelpers.CompareFiles("user-settings.json", _settings.SettingsFilePath);
        }

        [TestMethod]
        public void ListSettingsTest()
        {
            _settings.Location = Location;

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new SettingsCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "list" },
                        Settings = _settings
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "settings-list.txt", 1);
        }

        [TestMethod]
        public void ClearSettingsTest()
        {
            _settings.Location = Location;

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new SettingsCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.Interactive,
                        Arguments = new string[] { "clear" },
                        Settings = _settings
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "settings-clear.txt", 1);
        }
    }
}
