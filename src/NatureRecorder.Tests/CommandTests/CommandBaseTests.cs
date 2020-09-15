using System.IO;
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
            string data;
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new AddCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "category" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            Assert.IsTrue(data.Contains("is not valid for command mode"));
        }

        [TestMethod]
        public void InvalidArgumentCountTest()
        {
            string data;
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new AddCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { "user" }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            Assert.IsTrue(data.Contains("Incorrect argument count"));
        }
    }
}
