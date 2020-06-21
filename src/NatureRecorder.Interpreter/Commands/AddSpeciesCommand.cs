using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddSpeciesCommand : CommandBase
    {
        public AddSpeciesCommand()
        {
            Type = CommandType.addspecies;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                Category category = ReadCategory(context);
                if (category != null)
                {
                    string speciesName = ReadSpeciesName(context);
                    if (speciesName != null)
                    {
                        Species species = context.Factory.Species.Add(speciesName, category.Name);
                        context.Output.WriteLine($"Species {species.Name} added to {category.Name}");
                        context.Output.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Prompt for a new location name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Category ReadCategory(CommandContext context)
        {
            Category category = null;

            do
            {
                // Read the category name
                string name = context.Reader.ReadLine("Category name", false);
                if (!context.Reader.Cancelled)
                {
                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the location already exists. If so, print a warning
                    // and try again
                    category = context.Factory.Categories.Get(c => c.Name == name);
                    if (category == null)
                    {
                        context.Output.WriteLine($"Category {name} does not exist");
                    }
                }
            }
            while (!context.Reader.Cancelled && (category == null));

            return category;
        }

        /// <summary>
        /// Prompt for a new location name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ReadSpeciesName(CommandContext context)
        {
            Species existing;
            string name;

            do
            {
                // Reset
                existing = null;

                // Read the location name
                name = context.Reader.ReadLine("Species name", false);
                if (!context.Reader.Cancelled)
                {
                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the location already exists. If so, print a warning
                    // and try again
                    existing = context.Factory.Species.Get(s => s.Name == name);
                    if (existing != null)
                    {
                        context.Output.WriteLine($"Species {name} already exists");
                    }
                }
            }
            while (existing != null);

            return name;
        }
    }
}
