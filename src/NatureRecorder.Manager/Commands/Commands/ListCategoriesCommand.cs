using System;
using System.Collections.Generic;
using System.Linq;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class ListCategoriesCommand : CommandBase
    {
        public ListCategoriesCommand()
        {
            Type = CommandType.categories;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                IEnumerable<Category> categories = factory.Categories.List(null, 1, int.MaxValue);
                if (categories.Any())
                {
                    Console.WriteLine($"There are {categories.Count()} species categories in the database:\n");
                    foreach (Category category in categories.OrderBy(l => l.Name))
                    {
                        Console.WriteLine($"\t{category.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("There are no species categories in the database");
                }
            }
        }
    }
}
