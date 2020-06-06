using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Reporting;
using NatureRecorder.Manager.Entities;

namespace NatureRecorder.Manager.Logic
{
    public class CommandRunner
    {
        private const int AllSightings = 99999999;

        private readonly NatureRecorderDbContext _context;
        private readonly NatureRecorderFactory _factory;

        public CommandRunner()
        {
            _context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            _factory = new NatureRecorderFactory(_context);
        }

        /// <summary>
        /// Execute the operations represented by the specified operation instance
        /// </summary>
        /// <param name="op"></param>
        public void Run(Operation op)
        {
            try
            {
                switch (op.Type)
                {
                    case OperationType.add:
                        AddUser(op);
                        break;
                    case OperationType.setpassword:
                        SetPassword(op);
                        break;
                    case OperationType.delete:
                        DeleteUser(op);
                        break;
                    case OperationType.check:
                        CheckImportFile(op);
                        break;
                    case OperationType.import:
                        Import(op);
                        break;
                    case OperationType.export:
                        Export(op);
                        break;
                    case OperationType.summary:
                    case OperationType.report:
                        Summarise(op);
                        break;
                    case OperationType.update:
                        UpdateDatabase();
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

        /// <summary>
        /// Add a user to the database
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="op"></param>
        private void AddUser(Operation op)
        {
            _factory.Users.AddUser(op.UserName, op.Password);
            Console.WriteLine($"Added user {op.UserName}");
        }

        /// <summary>
        /// Set a users password
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="op"></param>
        private void SetPassword(Operation op)
        {
            _factory.Users.SetPassword(op.UserName, op.Password);
            Console.WriteLine($"Set password for user {op.UserName}");
        }

        /// <summary>
        /// Remove a user from the database
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="op"></param>
        private void DeleteUser(Operation op)
        {
            _factory.Users.DeleteUser(op.UserName);
            Console.WriteLine($"Deleted user {op.UserName}");
        }

        /// <summary>
        /// Check an import file for potential typos
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="op"></param>
        private void CheckImportFile(Operation op)
        {
            _factory.Import.DetectNewLookups(op.FileName);
            _factory.Import.WriteNewLookupsToConsole();
        }

        /// <summary>
        /// Import data from a file
        /// </summary>
        /// <param name="op"></param>
        private void Import(Operation op)
        {
            // Check for new locations, species and categories first - these
            // may be genuine or may be typos in the data
            _factory.Import.DetectNewLookups(op.FileName);
            _factory.Import.WriteNewLookupsToConsole();

            // Confirm import of there are any new locations, species or categories
            bool import = true;
            if (_factory.Import.NewCategories.Any() ||
                _factory.Import.NewSpecies.Any() ||
                _factory.Import.NewLocations.Any())
            {
                import = PromptForYesNo("\nDo you want to import this file?");
            }

            // If there are no duplicates or the user wants to import, complete the import
            if (import)
            {
                _factory.Import.RecordImport += OnRecordImportExport;
                _factory.Import.Import(op.FileName);
                Console.WriteLine($"\nImported data from {op.FileName}");
            }
        }

        /// <summary>
        /// Export the database to a CSV file
        /// </summary>
        /// <param name="op"></param>
        private void Export(Operation op)
        {
            // The third parameter is an arbitrary large number intended to capture all
            // sightings
            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, AllSightings);
            _factory.Export.RecordExport += OnRecordImportExport;
            _factory.Export.Export(sightings, op.FileName);
            Console.WriteLine($"\nExported the database to {op.FileName}");
        }

        /// <summary>
        /// Summarise the sightings for the specified date/date range
        /// </summary>
        /// <param name="op"></param>
        private void Summarise(Operation op)
        {
            // Retrieve the summary for the specified date
            Summary summary = _factory.Sightings.Summarise(op.From, op.To);
            if (summary.Sightings.Any())
            {
                Console.WriteLine($"Summary of sightings from {op.From.ToString("dd-MMM-yyyy")} to {op.To.ToString("dd-MMM-yyyy")}:\n");

                string sightingsSuffix = (summary.Sightings.Count() > 1) ? "s" : "";
                Console.WriteLine($"\t{summary.Sightings.Count()} sighting{sightingsSuffix}");
                Console.WriteLine($"\t{summary.Species.Count()} species");

                string categoriesSuffix = (summary.Categories.Count() > 1) ? "ies" : "y";
                Console.WriteLine($"\t{summary.Categories.Count()} categor{categoriesSuffix}");

                string locationsSuffix = (summary.Locations.Count() > 1) ? "s" : "";
                Console.WriteLine($"\t{summary.Locations.Count()} location{locationsSuffix}\n");

                SightingsTable table = new SightingsTable(summary.Sightings);
                table.PrintTable();
            }
            else
            {
                Console.WriteLine($"There were no sightings on {op.From.ToShortDateString()}");
            }
        }

        /// <summary>
        /// Apply the latest database migrations
        /// </summary>
        private void UpdateDatabase()
        {
            _context.Database.Migrate();
            Console.WriteLine($"Applied the latest database migrations");
        }

        /// <summary>
        /// Callback handler for record import/export events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordImportExport(object sender, SightingDataExchangeEventArgs e)
        {
            Console.WriteLine($"{e.RecordCount} : {e.Sighting.Date.ToShortDateString()} {e.Sighting.Species.Name} {e.Sighting.Location.Name}");
        }

        /// <summary>
        /// Prompt for a y/Y or n/N keypress
        /// </summary>
        /// <returns></returns>
        private bool PromptForYesNo(string prompt)
        {
            bool? yesNo = null;

            Console.Write($"{prompt} [y/n] : ");

            do
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                switch (info.KeyChar.ToString().ToUpper())
                {
                    case "Y":
                        Console.WriteLine("y");
                        yesNo = true;
                        break;
                    case "N":
                        Console.WriteLine("n");
                        yesNo = false;
                        break;
                    default:
                        break;
                }
            }
            while (yesNo == null);

            return yesNo ?? false;
        }
    }
}
