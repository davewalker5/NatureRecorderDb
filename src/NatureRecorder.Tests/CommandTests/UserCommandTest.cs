using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;
using NatureRecorder.Tests.Helpers;

namespace NatureRecorder.Tests.CommandTests
{
    [TestClass]
    public class UserCommandTest
    {
        private const string UserName = "someuser";
        private const string Password = "password";
        private const string UpdatedPassword = "updated";

        private NatureRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new NatureRecorderFactory(context);
        }

        [TestMethod]
        public void AddUserCommandTest()
        {
            Assert.IsFalse(_factory.Context.Users.Any());

            string[] arguments = new string[] { "user", UserName, Password };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.CommandLine, null, null, null, null, 0);

            Assert.AreEqual(1, _factory.Context.Users.Count());

            int id = _factory.Context.Users.First().Id;
            User user = _factory.Users.GetUser(id);
            Assert.AreEqual(UserName, user.UserName);
            Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));
        }

        [TestMethod]
        public void SetPasswordCommandTest()
        {
            string[] arguments = new string[] { "user", UserName, Password };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.CommandLine, null, null, null, null, 0);

            Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));
            Assert.IsFalse(_factory.Users.Authenticate(UserName, UpdatedPassword));

            arguments = new string[] { UserName, UpdatedPassword };
            TestHelpers.RunCommand(_factory, arguments, new SetPasswordCommand(), CommandMode.CommandLine, null, null, null, null, 0);

            Assert.IsFalse(_factory.Users.Authenticate(UserName, Password));
            Assert.IsTrue(_factory.Users.Authenticate(UserName, UpdatedPassword));
        }

        [TestMethod]
        public void DeleteUserCommandTest()
        {
            string[] arguments = new string[] { "user", UserName, Password };
            TestHelpers.RunCommand(_factory, arguments, new AddCommand(), CommandMode.CommandLine, null, null, null, null, 0);

            Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));

            arguments = new string[] { "user", UserName };
            TestHelpers.RunCommand(_factory, arguments, new DeleteCommand(), CommandMode.CommandLine, null, null, null, null, 0);

            Assert.IsFalse(_factory.Context.Users.Any());
        }
    }
}
