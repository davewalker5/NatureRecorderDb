using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class ConnectionCommandTest
    {
        private const string ExpectedConnection = "Data Source=/Users/Wildlife/Data/naturerecorder.db";

        [TestMethod]
        public void ShowConnectionCommandTest()
        {
            string data = TestHelpers.RunCommand(null, null, new ConnectionCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.AreEqual(ExpectedConnection, data.Replace("\n", ""));
        }
    }
}
