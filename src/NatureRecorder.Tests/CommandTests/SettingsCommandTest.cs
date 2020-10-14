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

        private const string SettingsFile = "naturerecorder.settings";
        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(LocationDoesNotExistException))]
        public void SetInvalidLocationTest()
        {
            string[] arguments = new string[] { "location", Location };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownSettingTypeException))]
        public void RequestInvalidActionTest()
        {
            string[] arguments = new string[] { "notvalid" };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, null, 0);
        }

        [TestMethod]
        public void SetLocationTest()
        {
            _factory.Locations.Add(Location, "", "", "", "", "", null, null);

            string[] arguments = new string[] { "location", Location };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, null, 0);

            TestHelpers.CompareFiles("user-settings.json", SettingsFile);
        }

        [TestMethod]
        public void ListSettingsTest()
        {
            _factory.Locations.Add(Location, "", "", "", "", "", null, null);

            string[] arguments = new string[] { "location", Location };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, null, 0);

            arguments = new string[] { "list" };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, "settings-list.txt", 0);
        }

        [TestMethod]
        public void ClearSettingsTest()
        {
            _factory.Locations.Add(Location, "", "", "", "", "", null, null);

            string[] arguments = new string[] { "location", Location };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, null, 0);

            arguments = new string[] { "clear" };
            TestHelpers.RunCommand(_factory, arguments, new SettingsCommand(), CommandMode.Interactive, null, SettingsFile, null, "settings-clear.txt", 0);
        }
    }
}
