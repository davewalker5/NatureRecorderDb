using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ImportCommand : DataExchangeCommandBase
    {
        public ImportCommand()
        {
            Type = CommandType.import;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument is the export type
                DataExchangeType type = GetDataExchangeType(context);
                switch (type)
                {
                    case DataExchangeType.Sightings:
                        ImportSightings(context);
                        break;
                    case DataExchangeType.Status:
                        ImportSpeciesStatusRatings(context);
                        break;
                    default:
                        string message = "Cannot import data for unknown import type";
                        throw new UnknownDataExchangeTypeException(message);
                }
            }
        }

        /// <summary>
        /// Import as set of sightings 
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ImportSightings(CommandContext context)
        {
            // Check for new locations, species and categories first - these
            // may be genuine or may be typos in the data
            context.Factory.SightingsImport.DetectNewLookups(context.Arguments[1]);
            context.Factory.SightingsImport.WriteNewLookupsToStream(context.Output);
            context.Output.Flush();

            // Confirm import of there are any new locations, species or categories
            bool import = true;
            if (context.Factory.SightingsImport.NewCategories.Any() ||
                context.Factory.SightingsImport.NewSpecies.Any() ||
                context.Factory.SightingsImport.NewLocations.Any())
            {
                import = context.Reader.PromptForYesNo("\nDo you want to import this file?", 'N');
            }

            // If there are no duplicates or the user wants to import, complete the import
            if (import)
            {
                context.Factory.SightingsImport.RecordImport += OnSightingRecordImportExport;
                context.Factory.SightingsImport.Import(context.Arguments[1]);
                context.Output.WriteLine($"\nImported data from {context.Arguments[1]}");
                context.Output.Flush();
            }
        }

        /// <summary>
        /// Import as set of sightings 
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void ImportSpeciesStatusRatings(CommandContext context)
        {
            // Check for new species, categories, schemes and ratins first - these
            // may be genuine or may be typos in the data
            context.Factory.SpeciesStatusImport.DetectNewLookups(context.Arguments[1]);
            context.Factory.SpeciesStatusImport.WriteNewLookupsToStream(context.Output);
            context.Output.Flush();

            // Confirm import of there are any new locations, species or categories
            bool import = true;
            if (context.Factory.SpeciesStatusImport.NewCategories.Any() ||
                context.Factory.SpeciesStatusImport.NewSpecies.Any() ||
                context.Factory.SpeciesStatusImport.NewSchemes.Any() ||
                context.Factory.SpeciesStatusImport.NewRatings.Any())
            {
                import = context.Reader.PromptForYesNo("\nDo you want to import this file?", 'N');
            }

            // If there are no duplicates or the user wants to import, complete the import
            if (import)
            {
                context.Factory.SpeciesStatusImport.RecordImport += OnSpeciesStatusRecordImportExport;
                context.Factory.SpeciesStatusImport.Import(context.Arguments[1]);
                context.Output.WriteLine($"\nImported data from {context.Arguments[1]}");
                context.Output.Flush();
            }
        }
    }
}