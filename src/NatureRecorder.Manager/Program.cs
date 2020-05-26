using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Manager.Entities;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager
{
    class Program
    {
        /// <summary>
        /// Callback handler for record import/export events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnRecordImportExport(object sender, SightingDataExchangeEventArgs e)
        {
            Console.WriteLine($"{e.RecordCount} : {e.Sighting.Species.Name}, {e.Sighting.Species.Category.Name}, {e.Sighting.Location.Name}, {e.Sighting.Date.ToShortDateString()}");
        }

        static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Nature Recorder Database Management {version}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
                NatureRecorderFactory factory = new NatureRecorderFactory(context);

                try
                {
                    switch (op.Type)
                    {
                        case OperationType.add:
                            factory.Users.AddUser(op.UserName, op.Password);
                            Console.WriteLine($"Added user {op.UserName}");
                            break;
                        case OperationType.setpassword:
                            factory.Users.SetPassword(op.UserName, op.Password);
                            Console.WriteLine($"Set password for user {op.UserName}");
                            break;
                        case OperationType.delete:
                            factory.Users.DeleteUser(op.UserName);
                            Console.WriteLine($"Deleted user {op.UserName}");
                            break;
                        case OperationType.check:
                            factory.Import.DetectNewLookups(op.FileName);
                            factory.Import.WriteNewLookupsToConsole();
                            break;
                        case OperationType.import:
                            factory.Import.RecordImport += OnRecordImportExport;
                            factory.Import.Import(op.FileName);
                            Console.WriteLine($"Imported data from {op.FileName}");
                            break;
                        case OperationType.export:
                            // The third parameter is an arbitrary large number intended to capture all
                            // sightings
                            IEnumerable<Sighting> sightings = factory.Sightings.List(null, 1, 99999999);
                            factory.Export.RecordExport += OnRecordImportExport;
                            factory.Export.Export(sightings, op.FileName);
                            Console.WriteLine($"Exported the database to {op.FileName}");
                            break;
                        case OperationType.update:
                            context.Database.Migrate();
                            Console.WriteLine($"Applied the latest database migrations");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error : {ex.Message}");
                }
            }
            else
            {
                string executable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("Usage:");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} check csv_file_path");
                Console.WriteLine($"[5] {executable} import csv_file_path");
                Console.WriteLine($"[6] {executable} export csv_file_path");
                Console.WriteLine($"[7] {executable} update");
            }
        }
    }
}
