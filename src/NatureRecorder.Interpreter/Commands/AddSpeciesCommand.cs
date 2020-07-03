using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddSpeciesCommand : AddCommandBase
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
                Category category = PromptForCategory(context, true);
                if (category != null)
                {
                    Species template = PromptForSpecies(context, category.Id, false);
                    if (template != null)
                    {
                        Species species = context.Factory.Species.Add(template.Name, category.Name);
                        context.Output.WriteLine($"Species {species.Name} added to {category.Name}");
                        context.Output.Flush();
                    }
                }
            }
        }
    }
}
