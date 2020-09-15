using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Commands;
using NatureRecorder.Interpreter.Entities;

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

            using (StreamWriter output = new StreamWriter(new MemoryStream()))
            {
                new AddCommand().Run(new CommandContext
                {
                    Output = output,
                    Factory = _factory,
                    Mode = CommandMode.CommandLine,
                    Arguments = new string[] { "user", UserName, Password }
                });
            }

            Assert.AreEqual(1, _factory.Context.Users.Count());

            int id = _factory.Context.Users.First().Id;
            User user = _factory.Users.GetUser(id);
            Assert.AreEqual(UserName, user.UserName);
            Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));
        }

        [TestMethod]
        public void SetPasswordCommandTest()
        {
            using (StreamWriter output = new StreamWriter(new MemoryStream()))
            {
                new AddCommand().Run(new CommandContext
                {
                    Output = output,
                    Factory = _factory,
                    Mode = CommandMode.CommandLine,
                    Arguments = new string[] { "user", UserName, Password }
                });

                Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));
                Assert.IsFalse(_factory.Users.Authenticate(UserName, UpdatedPassword));

                new SetPasswordCommand().Run(new CommandContext
                {
                    Output = output,
                    Factory = _factory,
                    Mode = CommandMode.CommandLine,
                    Arguments = new string[] { UserName, UpdatedPassword }
                });

                Assert.IsFalse(_factory.Users.Authenticate(UserName, Password));
                Assert.IsTrue(_factory.Users.Authenticate(UserName, UpdatedPassword));
            }
        }

        [TestMethod]
        public void DeleteUserCommandTest()
        {
            using (StreamWriter output = new StreamWriter(new MemoryStream()))
            {
                new AddCommand().Run(new CommandContext
                {
                    Output = output,
                    Factory = _factory,
                    Mode = CommandMode.CommandLine,
                    Arguments = new string[] { "user", UserName, Password }
                });

                Assert.IsTrue(_factory.Users.Authenticate(UserName, Password));

                new DeleteCommand().Run(new CommandContext
                {
                    Output = output,
                    Factory = _factory,
                    Mode = CommandMode.CommandLine,
                    Arguments = new string[] { "user", UserName }
                });

                Assert.IsFalse(_factory.Context.Users.Any());
            }
        }
    }
}
