using System;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ImportCommand : DataExchangeCommandBase
    {
        public ImportCommand()
        {
            Type = CommandType.import;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // Check for new locations, species and categories first - these
                // may be genuine or may be typos in the data
                factory.Import.DetectNewLookups(arguments[0]);
                factory.Import.WriteNewLookupsToConsole();

                // Confirm import of there are any new locations, species or categories
                bool import = true;
                if (factory.Import.NewCategories.Any() ||
                    factory.Import.NewSpecies.Any() ||
                    factory.Import.NewLocations.Any())
                {
                    import = PromptForYesNo("\nDo you want to import this file?");
                }

                // If there are no duplicates or the user wants to import, complete the import
                if (import)
                {
                    factory.Import.RecordImport += OnRecordImportExport;
                    factory.Import.Import(arguments[0]);
                    Console.WriteLine($"\nImported data from {arguments[0]}");
                }
            }
        }

        /// <summary>
        /// Prompt for a y/Y or n/N keypress
        /// </summary>
        /// <returns></returns>
        protected bool PromptForYesNo(string prompt)
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