using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ExportCommand : DataExchangeCommandBase
    {
        public ExportCommand()
        {
            Type = CommandType.export;
            MinimumArguments = 2;
            MaximiumArguments = 6;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument is the export type, that determines the
                // required argument count
                DataExchangeType type = GetDataExchangeType(context);
                if (ArgumentCountCorrectForDataExchangeType(context, type))
                {
                    switch (type)
                    {
                        case DataExchangeType.All:
                            ExportAll(context);
                            break;
                        case DataExchangeType.Location:
                            ExportLocationData(context);
                            break;
                        case DataExchangeType.Category:
                            ExportCategoryData(context);
                            break;
                        case DataExchangeType.Species:
                            ExportSpeciesData(context);
                            break;
                        case DataExchangeType.Status:
                            ExportSpeciesStatusData(context);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Export conservation status ratings
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ExportSpeciesStatusData(CommandContext context)
        {
            // export status [file]
            IEnumerable<SpeciesStatusRating> ratings = GetRatings(context);
            ExportSpeciesStatusRatings(context, 1, ratings);
        }

        /// <summary>
        /// Export sightings for a specified species, location and date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ExportSpeciesData(CommandContext context)
        {
            // export species [name] [location] [file] <from> <to>
            IEnumerable<Sighting> sightings = GetSightings(context, 4, 5, 2, -1, 1);
            ExportSightings(context, 3, sightings);
        }

        /// <summary>
        /// Export sightings for a specified category, location and date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ExportCategoryData(CommandContext context)
        {
            // export category [name] [location] [file] <from> <to>
            IEnumerable<Sighting> sightings = GetSightings(context, 4, 5, 2, 1, -1);
            ExportSightings(context, 3, sightings);
        }

        /// <summary>
        /// Export sightings for a specified location and date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ExportLocationData(CommandContext context)
        {
            // export location [name] [file] <from> <to>
            IEnumerable<Sighting> sightings = GetSightings(context, 3, 4, 1, -1, -1);
            ExportSightings(context, 2, sightings);
        }

        /// <summary>
        /// Show the summary report for a specified date range
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ExportAll(CommandContext context)
        {
            // export all [file] <from> <to>
            IEnumerable<Sighting> sightings = GetSightings(context, 2, 3, -1, -1, -1);
            ExportSightings(context, 1, sightings);
        }

        /// <summary>
        /// Return a lit of species conservation status ratings matching the specified
        /// criteria (none in the initial implementation)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IEnumerable<SpeciesStatusRating> GetRatings(CommandContext context)
        {
            IEnumerable<SpeciesStatusRating> ratings = context.Factory.SpeciesStatusRatings.List(null, 1, int.MaxValue);
            return ratings;
        }

        /// <summary>
        /// Return a list of sightings in the date range specified in the arguments
        /// with the specified indices
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fromDateIndex"></param>
        /// <param name="toDateIndex"></param>
        /// <param name="locationIndex"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="speciesIndex"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IEnumerable<Sighting> GetSightings(CommandContext context, int fromDateIndex, int toDateIndex, int locationIndex, int categoryIndex, int speciesIndex)
        {
            // Extract the filtering properties from the arguments
            int? locationId = GetLocation(context, locationIndex);
            int? categoryId = GetCategory(context, categoryIndex);
            int? speciesId = GetSpecies(context, speciesIndex);
            DateTime? from = GetDateFromArgument(context.Arguments, fromDateIndex, null, context.Output);
            DateTime? to = GetDateFromArgument(context.Arguments, toDateIndex, null, context.Output);

            // Query based on the date range, that may have null start or end dates
            IEnumerable<Sighting> sightings;
            if ((from == null) && (to == null))
            {
                sightings = context.Factory.Sightings.List(null, 1, int.MaxValue);
            }
            else if (to == null)
            {
                DateTime reportFrom = from ?? DateTime.Now;
                sightings = context.Factory.Sightings.List(s => s.Date >= reportFrom, 1, int.MaxValue);
            }
            else
            {
                DateTime reportFrom = from ?? DateTime.Now;
                DateTime reportTo = to ?? DateTime.Now;
                sightings = context.Factory.Sightings.ListByDate(reportFrom, reportTo, 1, int.MaxValue);
            }

            // Appply location, category and species filtering
            if (locationId != null)
            {
                sightings = sightings.Where(s => s.LocationId == locationId);
            }

            if (categoryId != null)
            {
                sightings = sightings.Where(s => s.Species.CategoryId == categoryId);
            }

            if (speciesId != null)
            {
                sightings = sightings.Where(s => s.SpeciesId == speciesId);
            }

            return sightings;
        }

        /// <summary>
        /// Export a collection of sightings to the file specified in the context
        /// arguments
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileNameIndex"></param>
        /// <param name="sightings"></param>
        [ExcludeFromCodeCoverage]
        private void ExportSightings(CommandContext context, int fileNameIndex, IEnumerable<Sighting> sightings)
        {
            try
            {
                context.Factory.SightingsExport.RecordExport += OnSightingRecordImportExport;
                context.Factory.SightingsExport.Export(sightings, context.Arguments[fileNameIndex]);
                context.Output.WriteLine($"\nExported the database to {context.Arguments[fileNameIndex]}");
                context.Output.Flush();
            }
            catch
            {
                context.Factory.SightingsExport.RecordExport -= OnSightingRecordImportExport;
                throw;
            }
        }

        /// <summary>
        /// Export a collection of conservation status ratings to the file specified
        /// in the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileNameIndex"></param>
        /// <param name="ratings"></param>
        [ExcludeFromCodeCoverage]
        private void ExportSpeciesStatusRatings(CommandContext context, int fileNameIndex, IEnumerable<SpeciesStatusRating> ratings)
        {
            try
            {
                context.Factory.SpeciesStatusExport.RecordExport += OnSpeciesStatusRecordImportExport;
                context.Factory.SpeciesStatusExport.Export(ratings, context.Arguments[fileNameIndex]);
                context.Output.WriteLine($"\nExported the conservation status ratings to {context.Arguments[fileNameIndex]}");
                context.Output.Flush();
            }
            catch
            {
                context.Factory.SpeciesStatusExport.RecordExport -= OnSpeciesStatusRecordImportExport;
                throw;
            }
        }

        /// <summary>
        /// Check the argument count is correct for the specified export type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool ArgumentCountCorrectForDataExchangeType(CommandContext context, DataExchangeType type)
        {
            (int minimum, int maximum) counts = GetRequiredArgumentCounts(type);
            bool correct = ((context.Arguments.Length >= counts.minimum) && (context.Arguments.Length <= counts.maximum));
            if (!correct)
            {
                context.Output.WriteLine($"Incorrect argument count for the \"{Type} {context.Arguments[0]}\" command");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Return the argument limits based on the export type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private (int minimum, int maximum) GetRequiredArgumentCounts(DataExchangeType type)
        {
            (int minimum, int maximum) counts;

            switch (type)
            {
                case DataExchangeType.All:
                    counts.minimum = 2;
                    counts.maximum = 4;
                    break;
                case DataExchangeType.Location:
                    counts.minimum = 3;
                    counts.maximum = 5;
                    break;
                case DataExchangeType.Category:
                    counts.minimum = 4;
                    counts.maximum = 6;
                    break;
                case DataExchangeType.Species:
                    counts.minimum = 4;
                    counts.maximum = 6;
                    break;
                case DataExchangeType.Status:
                    counts.minimum = 2;
                    counts.maximum = 2;
                    break;
                default:
                    string message = "Cannot export data for unknown export type";
                    throw new UnknownDataExchangeTypeException(message);
            }

            return counts;
        }
    }
}