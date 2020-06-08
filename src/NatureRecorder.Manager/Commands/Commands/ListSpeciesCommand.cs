using System;
using System.Collections.Generic;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ListSpeciesCommand : CommandBase
    {
        public ListSpeciesCommand()
        {
            Type = CommandType.species;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                // Get the category details from the database
                string categoryName = ToTitleCase(arguments[0]);
                Category category = factory.Categories.Get(c => c.Name == categoryName);
                if (category != null)
                {
                    // Get a list of species in that category
                    IEnumerable<Species> matches = factory.Species
                                                          .List(s => s.CategoryId == category.Id, 1, int.MaxValue);
                    if (matches.Any())
                    {
                        // Show the results
                        Console.WriteLine($"There are {matches.Count()} species listed for category \"{categoryName}\":\n");
                        foreach (Species species in matches.OrderBy(l => l.Name))
                        {
                            Console.WriteLine($"\t{species.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"There are no species for category \"{categoryName}\" in the database");
                    }
                }
                else
                {
                    Console.WriteLine($"Species category \"{categoryName}\" does not exist");
                }
            }
        }
    }
}
