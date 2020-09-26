using System.Linq;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ImportCommand : DataExchangeCommandBase
    {
        public ImportCommand()
        {
            Type = CommandType.import;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // Check for new locations, species and categories first - these
                // may be genuine or may be typos in the data
                context.Factory.Import.DetectNewLookups(context.Arguments[0]);
                context.Factory.Import.WriteNewLookupsToStream(context.Output);
                context.Output.Flush();

                // Confirm import of there are any new locations, species or categories
                bool import = true;
                if (context.Factory.Import.NewCategories.Any() ||
                    context.Factory.Import.NewSpecies.Any() ||
                    context.Factory.Import.NewLocations.Any())
                {
                    import = context.Reader.PromptForYesNo("\nDo you want to import this file?", 'N');
                }

                // If there are no duplicates or the user wants to import, complete the import
                if (import)
                {
                    context.Factory.Import.RecordImport += OnRecordImportExport;
                    context.Factory.Import.Import(context.Arguments[0]);
                    context.Output.WriteLine($"\nImported data from {context.Arguments[0]}");
                    context.Output.Flush();
                }
            }
        }
    }
}