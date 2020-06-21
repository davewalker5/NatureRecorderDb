using System.Collections.Generic;
using System.Linq;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class ListCategoriesCommand : CommandBase
    {
        public ListCategoriesCommand()
        {
            Type = CommandType.categories;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                IEnumerable<Category> categories = context.Factory.Categories.List(null, 1, int.MaxValue);
                if (categories.Any())
                {
                    context.Output.WriteLine($"There are {categories.Count()} species categories in the database:\n");
                    foreach (Category category in categories.OrderBy(l => l.Name))
                    {
                        context.Output.WriteLine($"\t{category.Name}");
                    }
                }
                else
                {
                    context.Output.WriteLine("There are no species categories in the database");
                }

                context.Output.Flush();
            }
        }
    }
}
