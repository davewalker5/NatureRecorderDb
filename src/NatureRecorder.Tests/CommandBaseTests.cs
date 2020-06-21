using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NatureRecorder.Tests
{
    [TestClass]
    public class CommandBaseTests
    {
        private string _currentFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            _currentFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ImportExportManagerTest)).Location);
        }

        [TestMethod]
        public void InvalidCommandModeTest()
        {

        }

        [TestMethod]
        public void InvalidArgumentCountTest()
        {

        }
    }
}
