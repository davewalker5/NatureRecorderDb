using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests
{
    [TestClass]
    public class HelpCommandTest
    {
        [TestMethod]
        public void ShowHelpCommandTest()
        {
            string data;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    new HelpCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            TestHelpers.CompareOutput(data, "help.txt");
        }
    }
}
