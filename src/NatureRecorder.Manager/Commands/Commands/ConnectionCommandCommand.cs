using System;
using Microsoft.Extensions.Configuration;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ConnectionCommand : CommandBase
    {
        public ConnectionCommand()
        {
            Type = CommandType.connection;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // Get the configuration string from the appsettings.json file and write it out
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                        .AddJsonFile("appsettings.json")
                                                        .Build();

                string connectionString = configuration.GetConnectionString("NatureRecorderDB");
                Console.WriteLine($"{connectionString}");
            }
        }
    }
}