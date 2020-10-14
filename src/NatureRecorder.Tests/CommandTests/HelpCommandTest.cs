using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class HelpCommandTest
    {
        [TestMethod]
        public void ShowHelpCommandTest()
        {
            string[] arguments = new string[] { "something" };
            TestHelpers.RunCommand(null, null, new HelpCommand(), CommandMode.CommandLine, null, null, null, "help.txt", 0);
        }
    }
}
