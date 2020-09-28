using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class UserSettingsTest
    {
        private const string Location = "My Default Location";

        [TestMethod]
        public void DefaultSettingsTest()
        {
            UserSettings settings = new UserSettings();
            Assert.IsFalse(string.IsNullOrEmpty(settings.SettingsFilePath));
            Assert.IsTrue(string.IsNullOrEmpty(settings.Location));
        }

        [TestMethod]
        public void LoadFromMissingFileTest()
        {
            UserSettings settings = new UserSettings("");
            settings.Load();
            Assert.IsTrue(string.IsNullOrEmpty(settings.Location));
        }

        [TestMethod]
        public void SaveAndLoadTest()
        {
            UserSettings settings = new UserSettings(Path.GetTempFileName());
            settings.Location = Location;
            settings.Save();

            UserSettings loaded = new UserSettings(settings.SettingsFilePath);
            loaded.Load();
            File.Delete(loaded.SettingsFilePath);

            Assert.AreEqual(Location, loaded.Location);
        }
    }
}
