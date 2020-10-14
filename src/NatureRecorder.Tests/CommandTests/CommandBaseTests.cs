using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class CommandBaseTests
    {
        [TestMethod]
        public void InvalidCommandModeTest()
        {
            string[] arguments = new string[] { "category" };
            string data = TestHelpers.RunCommand(null, arguments, new AddCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("is not valid for command mode"));
        }

        [TestMethod]
        public void TooFewArgumentsTest()
        {
            string data = TestHelpers.RunCommand(null, null, new ImportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("expects"));
        }

        [TestMethod]
        public void TooManyArgumentsTest()
        {
            string[] arguments = new string[] { "importtype", "filename", "extra" };
            string data = TestHelpers.RunCommand(null, arguments, new ImportCommand(), CommandMode.CommandLine, null, null, null, null, 0);
            Assert.IsTrue(data.Contains("expects"));
        }
    }
}
