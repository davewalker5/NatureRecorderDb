using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class AddCategoryCommand : CommandBase
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
                string categoryName = ReadName(context);
                if (categoryName != null)
                {
                    Category category = context.Factory.Categories.Add(categoryName);
                    context.Output.WriteLine($"Category {category.Name} added");
                    context.Output.Flush();
                }
            }
        }

        /// <summary>
        /// Prompt for a new location name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ReadName(CommandContext context)
        {
            Category existing;
            string name;

            do
            {
                // Reset
                existing = null;

                // Read the location name
                name = context.Reader.ReadLine("Category name", false);
                if (!context.Reader.Cancelled)
                {
                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the location already exists. If so, print a warning
                    // and try again
                    existing = context.Factory.Categories.Get(c => c.Name == name);
                    if (existing != null)
                    {
                        context.Output.WriteLine($"Category {name} already exists");
                    }
                }
            }
            while (existing != null);

            return name;
        }
    }
}
