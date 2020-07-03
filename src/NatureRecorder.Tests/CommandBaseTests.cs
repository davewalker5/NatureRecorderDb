﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests
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
                    new AddCategoryCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { }
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
                    new AddUserCommand().Run(new CommandContext
                    {
                        Output = output,
                        Mode = CommandMode.CommandLine,
                        Arguments = new string[] { }
                    });

                    data = TestHelpers.ReadStream(stream);
                }
            }

            Assert.IsTrue(data.Contains("expects between"));
        }
    }
}