using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class HistoryCommandTests
    {
        private const string Command = "list locations";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext dbContext = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(dbContext);
        }

        [TestMethod]
        public void ShowLocation()
        {
            string historyFile = Path.GetTempFileName();
            string[] arguments = new string[] { "location" };
            string data = TestHelpers.RunCommand(_factory, arguments, new HistoryCommand(), CommandMode.Interactive, historyFile, null, null, null, 0);
            Assert.AreEqual($"{historyFile}", data.Replace("\n", ""));
            File.Delete(historyFile);
        }

        [TestMethod]
        public void InitialHistoryIsEmpty()
        {
            string historyFile = Path.GetTempFileName();
            string data = TestHelpers.RunCommand(_factory, null, new HistoryCommand(), CommandMode.Interactive, historyFile, null, null, null, 0);
            Assert.AreEqual("History is empty", data.Replace("\n", ""));
            File.Delete(historyFile);
        }

        [TestMethod]
        public void ListHistory()
        {
            // We need to create a command interpreter to genuinely test adding to
            // the history but we can simulate it  by writing a command to the history
            // file and listing the history
            string historyFile = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(historyFile))
            {
                writer.WriteLine(Command);
            }

            string data = TestHelpers.RunCommand(_factory, null, new HistoryCommand(), CommandMode.Interactive, historyFile, null, null, null, 0);
            Assert.AreEqual($"     1 {Command}", data.Replace("\n", ""));
            File.Delete(historyFile);
        }

        [TestMethod]
        public void ClearHistory()
        {
            // We need to create a command interpreter to genuinely test adding to
            // the history but we can simulate it  by writing a command to the history
            // file and listing the history
            string historyFile = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(historyFile))
            {
                writer.WriteLine(Command);
            }

            // Clear the history
            string[] arguments = new string[] { "clear" };
            TestHelpers.RunCommand(_factory, arguments, new HistoryCommand(), CommandMode.Interactive, historyFile, null, null, null, 0);

            // And list it again
            string data = TestHelpers.RunCommand(_factory, null, new HistoryCommand(), CommandMode.Interactive, historyFile, null, null, null, 0);
            Assert.AreEqual("History is empty", data.Replace("\n", ""));
            File.Delete(historyFile);
        }
    }
}
