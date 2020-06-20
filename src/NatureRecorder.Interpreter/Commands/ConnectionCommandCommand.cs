using System;
using Microsoft.Extensions.Configuration;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ConnectionCommand : CommandBase
    {
        public ConnectionCommand()
        {
            Type = CommandType.connection;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(context))
            {
                // Get the configuration string from the appsettings.json file and write it out
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                        .AddJsonFile("appsettings.json")
                                                        .Build();

                string connectionString = configuration.GetConnectionString("NatureRecorderDB");
                context.Output.WriteLine($"{connectionString}");
                context.Output.Flush();
            }
        }
    }
}