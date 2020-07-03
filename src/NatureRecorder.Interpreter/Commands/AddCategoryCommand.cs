using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddCategoryCommand : AddCommandBase
    {
        public AddCategoryCommand()
        {
            Type = CommandType.addcategory;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                Category template = PromptForCategory(context, false);
                if (template != null)
                {
                    Category category = context.Factory.Categories.Add(template.Name);
                    context.Output.WriteLine($"Category {category.Name} added");
                    context.Output.Flush();
                }
            }
        }
    }
}
