using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.UnitTests
{
    [TestClass]
    public class CommandHistoryTest
    {
        private const string Command = "list locations";

        private string _fileName;
        private CommandHistory _history;

        [TestInitialize]
        public void TestInitialize()
        {
            _fileName = Path.GetTempFileName();
            _history = new CommandHistory(_fileName);
        }

        [TestMethod]
        public void HistoryFileTest()
        {
            Assert.AreEqual(_history.HistoryFile, _fileName);
        }

        [TestMethod]
        public void AddHistoryTest()
        {
            Assert.AreEqual(0, _history.Count);
            _history.Add(Command);
            Assert.AreEqual(1, _history.Count);
            Assert.AreEqual(Command, _history.Get(1));
        }

        [TestMethod]
        public void GetHistoryTest()
        {
            _history.Add(Command);
            string command = _history.Get(1);
            Assert.AreEqual(Command, command);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidHistoryEntryException))]
        public void GetEmptyHistoryTest()
        {
            _history.Get(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidHistoryEntryException))]
        public void GetInvalidHistoryTest()
        {
            _history.Add(Command);
            _history.Get(0);
        }

        [TestMethod]
        public void HistoryLocationTest()
        {
            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    _history.Location(output);
                    data = TestHelpers.ReadStream(stream).Replace("\n", "");
                }
            }

            Assert.AreEqual(_fileName, data);
        }

        [TestMethod]
        public void ListHistoryTest()
        {
            _history.Add(Command);

            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    _history.List(output);
                    data = TestHelpers.ReadStream(stream).Replace("\n", "");
                }
            }

            Assert.AreEqual($"     1 {Command}", data);
        }
    }
}
