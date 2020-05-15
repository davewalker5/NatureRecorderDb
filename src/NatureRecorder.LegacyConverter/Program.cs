using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NatureRecorder.LegacyConverter.Entities;
using NatureRecorder.LegacyConverter.Logic;

namespace NatureRecorder.LegacyConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            Console.WriteLine($"Nature Recorder Database Converter {versionInfo.FileVersion}");
            Console.WriteLine($"{versionInfo.LegalCopyright}\n");

            if (args.Length > 0)
            {
                // Read the application settings from config
                IConfiguration configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("appsettings.json")
                                                    .Build();

                IConfigurationSection section = configuration.GetSection("AppSettings");
                AppSettings settings = section.Get<AppSettings>();

                // Create a database converter and use it to read the database
                Console.WriteLine($"Reading database {args[0]}");
                Database db = new DbReader(settings).ReadDatabase(args[0]);
                Console.WriteLine($"Read {db.Records.Count} records");

                // Write the database to CSV
                Console.WriteLine($"Writing CSV file {db.File}.csv");
                new DbWriter().WriteDatabase(db);
            }
            else
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} database_file_path");
            }
        }
    }
}
