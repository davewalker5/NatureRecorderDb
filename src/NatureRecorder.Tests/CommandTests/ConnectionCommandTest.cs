using System.IO;
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
            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new ConnectionCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { }
                    });

                    data = TestHelpers.ReadStream(stream).Replace("\n", "");
                }
            }

            Assert.AreEqual(ExpectedConnection, data);
        }
    }
}
