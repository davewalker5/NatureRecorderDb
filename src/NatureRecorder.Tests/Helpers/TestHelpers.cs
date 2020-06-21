using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NatureRecorder.Tests.Helpers
{
    internal static class TestHelpers
    {
        private static string _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);

        /// <summary>
        /// Read the specified memory stream's content to a string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadStream(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Compare the specified data with the contents of the specified test
        /// output verification file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        public static void CompareOutput(string data, string file)
        {
            // Split the test results data, ignoring empty entries
            string[] dataLines = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Read the verification file and split it, again ignoring empty
            // entries
            string filePath = Path.Combine(_currentFolder, "Content", file);
            string fileContent = File.ReadAllText(filePath);
            string[] requiredLines = fileContent.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(requiredLines.Length, dataLines.Length);
            for (int i = 0; i < requiredLines.Length; i++)
            {
                Assert.AreEqual(requiredLines[i], dataLines[i]);
            }
        }
    }
}
