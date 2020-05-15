using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Manager.Entities;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Users
{
    class Program
    {
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
                        case OperationType.import:
                            factory.Import.Import(op.FileName);
                            Console.WriteLine($"Imported data from {op.FileName}");
                            break;
                        case OperationType.export:
                            // The third parameter is an arbitrary large number intended to capture all
                            // sightings
                            IEnumerable<Sighting> sightings = factory.Sightings.List(null, 1, 99999999);
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
                Console.WriteLine($"[4] {executable} import csv_file_path");
                Console.WriteLine($"[5] {executable} export csv_file_path");
                Console.WriteLine($"[6] {executable} update");
            }
        }
    }
}
