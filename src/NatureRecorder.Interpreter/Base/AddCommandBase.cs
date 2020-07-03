using System;
using System.Globalization;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class AddCommandBase : CommandBase
    {
        protected const string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Prompt for a date
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="expectExisting"></param>
        /// <returns></returns>
        protected DateTime? PromptForDate(CommandContext context, string defaultValue)
        {
            DateTime? date;
            DateTime parsedDate;
            bool done = false;
            string dateString;
            string prompt;

            // Construct the prompt
            bool allowBlank = !string.IsNullOrEmpty(defaultValue);
            if (string.IsNullOrEmpty(defaultValue))
            {
                prompt = "Date";
            }
            else
            {
                prompt = $"Date [{defaultValue}]";
            }

            do
            {
                // Reset
                date = null;

                // Read the date
                dateString = context.Reader.ReadLine(prompt, allowBlank);
                done = context.Reader.Cancelled;
                if (!done)
                {
                    // If the value's blank, use the supplied default
                    if (string.IsNullOrEmpty(dateString))
                    {
                        dateString = defaultValue;
                    }

                    // Attempt to parse the input as a date/time value
                    done = DateTime.TryParseExact(dateString, DateFormat, null, DateTimeStyles.None, out parsedDate);
                    if (done)
                    {
                        // Worked, so assign the return value
                        date = new DateTime?(parsedDate);
                    }
                    else
                    {
                        context.Output.WriteLine($"{dateString} is not a valid date");
                        context.Output.Flush();
                    }
                }
            }
            while (!done);

            return date;
        }

        /// <summary>
        /// Prompt for a location
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="expectExisting"></param>
        /// <returns></returns>
        protected Location PromptForLocation(CommandContext context, string defaultValue, bool expectExisting)
        {
            Location location;
            bool done = false;
            string name;
            string prompt;

            // Construct the prompt
            bool allowBlank = !string.IsNullOrEmpty(defaultValue);
            if (string.IsNullOrEmpty(defaultValue))
            {
                prompt = "Location name";
            }
            else
            {
                prompt = $"Location name [{defaultValue}]";
            }

            do
            {
                // Reset
                location = null;

                // Read the location name
                name = context.Reader.ReadLine(prompt, allowBlank);
                done = context.Reader.Cancelled;
                if (!done)
                {
                    // If the name's blank, use the supplied default
                    if (string.IsNullOrEmpty(name))
                    {
                        name = defaultValue;
                    }

                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the location already exists and generate an error
                    // depending on whether or not it should already exist
                    location = context.Factory.Locations.Get(l => l.Name == name);
                    if ((location == null) && expectExisting)
                    {
                        context.Output.WriteLine($"Location {name} does not exist");
                        context.Output.Flush();
                    }
                    else if ((location != null) && !expectExisting)
                    {
                        context.Output.WriteLine($"Location {name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
            while (!done);

            // If the location's NULL, create a new one containing the name
            if (!context.Reader.Cancelled && (location == null))
            {
                location = new Location { Name = name };
            }

            return location;
        }

        /// <summary>
        /// Prompt for a new location name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expectExisting"></param>
        /// <returns></returns>
        protected Category PromptForCategory(CommandContext context, bool expectExisting)
        {
            Category category;
            bool done = false;
            string name;

            do
            {
                // Reset
                category = null;

                // Read the location name
                name = context.Reader.ReadLine("Category name", false);
                done = context.Reader.Cancelled;
                if (!done)
                {
                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the category already exists and generate an error
                    // depending on whether or not it should already exist
                    category = context.Factory.Categories.Get(c => c.Name == name);
                    if ((category == null) && expectExisting)
                    {
                        context.Output.WriteLine($"Category {name} does not exist");
                        context.Output.Flush();
                    }
                    else if ((category != null) && !expectExisting)
                    {
                        context.Output.WriteLine($"Category {name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
            while (!done);

            // If the category's NULL, create a new one containing the name
            if (!context.Reader.Cancelled && (category == null))
            {
                category = new Category { Name = name };
            }

            return category;
        }

        /// <summary>
        /// Prompt for a new location name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="categoryId"></param>
        /// <param name="expectExisting"></param>
        /// <returns></returns>
        protected Species PromptForSpecies(CommandContext context, int? categoryId, bool expectExisting)
        {
            Species species;
            bool done = false;
            string name;

            do
            {
                // Reset
                species = null;

                // Read the location name
                name = context.Reader.ReadLine("Species name", false);
                done = context.Reader.Cancelled;
                if (!done)
                {
                    // Clean the string for comparison with existing locations
                    name = ToTitleCase(name);

                    // See if the species already exists and generate an error
                    // depending on whether or not it should already exist
                    species = context.Factory.Species.Get(s => s.Name == name);
                    if ((species == null) && expectExisting)
                    {
                        context.Output.WriteLine($"Species {name} does not exist");
                        context.Output.Flush();
                    }
                    else if ((species != null) && !expectExisting)
                    {
                        context.Output.WriteLine($"Species {name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        categoryId = species?.CategoryId;
                        done = true;
                    }
                }
            }
            while (!done);

            // If the species is NULL, create a new one containing the name
            if (!context.Reader.Cancelled && (species == null))
            {
                species = new Species { Name = name, CategoryId = categoryId ?? 0 };
            }

            return species;
        }
    }
}
