using System.Collections.Generic;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ListSpeciesCommand : CommandBase
    {
        public ListSpeciesCommand()
        {
            Type = CommandType.species;
            MinimumArguments = 1;
            MaximiumArguments = 1;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // Get the category details from the database
                string categoryName = ToTitleCase(context.Arguments[0]);
                Category category = context.Factory.Categories.Get(c => c.Name == categoryName);
                if (category != null)
                {
                    // Get a list of species in that category
                    IEnumerable<Species> matches = context.Factory
                                                          .Species
                                                          .List(s => s.CategoryId == category.Id, 1, int.MaxValue);
                    if (matches.Any())
                    {
                        // Show the results
                        context.Output.WriteLine($"There are {matches.Count()} species listed for category \"{categoryName}\":\n");
                        foreach (Species species in matches.OrderBy(l => l.Name))
                        {
                            context.Output.WriteLine($"\t{species.Name}");
                        }
                    }
                    else
                    {
                        context.Output.WriteLine($"There are no species for category \"{categoryName}\" in the database");
                    }
                }
                else
                {
                    context.Output.WriteLine($"Species category \"{categoryName}\" does not exist");
                }

                context.Output.Flush();
            }
        }
    }
}
