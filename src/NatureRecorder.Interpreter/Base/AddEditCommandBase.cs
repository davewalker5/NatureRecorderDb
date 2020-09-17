using System;
using System.Globalization;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class AddEditCommandBase : CommandBase
    {
        protected const string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Read a line of input, supporting a default value if the user just hits enter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isOptionalInput"></param>
        /// <returns></returns>
        protected (string input, bool cancelled) ReadLineWithDefaultValue(CommandContext context, string prompt, string defaultValue, bool isOptionalInput)
        {
            // Construct the prompt
            if (!string.IsNullOrEmpty(defaultValue))
            {
                prompt = $"{prompt} [{defaultValue}]";
            }

            // If the input is optional in the current context, blank input is always
            // allowed. Otherwise, whether or not blanks are allowed depends on whether
            // we have a non-empty default value (yes) or not (no)
            bool allowBlank = (isOptionalInput) ? true : !string.IsNullOrEmpty(defaultValue);

            // Read the user's input
            string input = context.Reader.ReadLine(prompt, allowBlank);
            bool cancelled = context.Reader.Cancelled;

            // If the user didn't cancel and the input's blank, replace it with
            // the default value
            if (!cancelled)
            {
                if (string.IsNullOrEmpty(input))
                {
                    input = defaultValue;
                }
            }

            return (input, cancelled);
        }

        /// <summary>
        /// Read a coordinate (latitude or longitude), supporting a default value
        /// if the user just hits enter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected (decimal? input, bool cancelled) ReadCoordinateWithDefaultValue(CommandContext context, string prompt, decimal? defaultValue)
        {
            // Construct the prompt
            if (defaultValue != null)
            {
                prompt = $"{prompt} [{defaultValue}]";
            }

            // Read the user's input
            decimal? input = context.Reader.Read<decimal?>(prompt);
            bool cancelled = context.Reader.Cancelled;

            // If the user didn't cancel and the input's null, replace it with
            // the default value
            if (!cancelled)
            {
                if (input == null)
                {
                    input = defaultValue;
                }
            }

            return (input, cancelled);
        }

        /// <summary>
        /// Prompt for a date
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="expectExisting"></param>
        /// <param name="isOptionalInput"></param>
        /// <returns></returns>
        protected DateTime? PromptForDate(CommandContext context, string defaultValue, bool isOptionalInput)
        {
            DateTime? date;
            bool inputValid;
            (string dateString, bool cancelled) input;

            do
            {
                // Reset
                date = null;
                inputValid = false;

                // Read the date
                input = ReadLineWithDefaultValue(context, "Date", defaultValue, isOptionalInput);
                if (!input.cancelled)
                {
                    // Attempt to parse the input as a date/time value
                    inputValid = DateTime.TryParseExact(input.dateString, DateFormat, null, DateTimeStyles.None, out DateTime parsedDate);
                    if (inputValid)
                    {
                        // Worked, so assign the return value
                        date = new DateTime?(parsedDate);
                    }
                    else
                    {
                        context.Output.WriteLine($"{input.dateString} is not a valid date");
                        context.Output.Flush();
                    }
                }
            }
            while (!inputValid && !input.cancelled);

            return date;
        }

        /// <summary>
        /// Prompt for the number of individuals seen
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isEditing"></param>
        /// <returns></returns>
        protected int? PromptForNumber(CommandContext context, string defaultValue, bool isEditing)
        {
            int? number;
            bool inputValid;
            (string numberString, bool cancelled) input;

            do
            {
                // Reset
                number = null;
                inputValid = false;

                // Read the number
                input = ReadLineWithDefaultValue(context, "Number", defaultValue, isEditing);
                if (!input.cancelled)
                {
                    // Attempt to parse the input as an integer
                    inputValid = int.TryParse(input.numberString, out int nonNullNumber);
                    if (inputValid && (nonNullNumber >= 0))
                    {
                        number = nonNullNumber;
                    }
                    else
                    {
                        context.Output.WriteLine($"{input.numberString} is not a valid number of individuals");
                        context.Output.Flush();
                    }
                }
            }
            while (!inputValid && !input.cancelled);

            return number;
        }

        /// <summary>
        /// Prompt for a location name and check that the corresponding location entity
        /// exists or not according to the parameters passed in
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="expectExisting"></param>
        /// <returns></returns>
        protected Location PromptForLocationByName(CommandContext context, string defaultValue, bool expectExisting)
        {
            Location location;
            bool inputValid;
            (string name, bool cancelled) input;

            do
            {
                // Reset
                location = null;
                inputValid = false;

                // Read the location name
                input = ReadLineWithDefaultValue(context, "Location", defaultValue, false);
                if (!input.cancelled)
                {
                    // Clean the string for comparison with existing locations and
                    // retrieve the locations with that name
                    input.name = ToTitleCase(input.name);
                    location = context.Factory.Locations.Get(l => l.Name == input.name);

                    // Check the returned location against the "expect existing" flag
                    if ((location == null) && expectExisting)
                    {
                        context.Output.WriteLine($"Location {input.name} does not exist");
                        context.Output.Flush();
                    }
                    else if ((location != null) && !expectExisting)
                    {
                        context.Output.WriteLine($"Location {input.name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        inputValid = true;
                    }
                }
            }
            while (!inputValid && !input.cancelled);

            // If the location's NULL, create a new one containing the name
            if (!context.Reader.Cancelled && (location == null))
            {
                location = new Location { Name = ToTitleCase(input.name) };
            }

            return location;
        }

        /// <summary>
        /// Prompt for a location name in the context of editing an existing location
        /// and confirm the name that's entered is valid in that context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <param name="existingLocationId"></param>
        /// <returns></returns>
        protected (string input, bool cancelled) PromptForLocationNameWhileEditingLocation(CommandContext context, string defaultValue, int existingLocationId)
        {
            bool inputValid;
            (string name, bool cancelled) input;

            do
            {
                // Reset
                inputValid = false;

                // Read the location name
                input = ReadLineWithDefaultValue(context, "Location", defaultValue, true);
                if (!input.cancelled)
                {
                    // Clean the string for comparison with existing locations and
                    // retrieve the locations with that name
                    input.name = ToTitleCase(input.name);
                    Location location = context.Factory.Locations.Get(l => l.Name == input.name);

                    // Either the location must not exist OR it's ID must be the one passed in.
                    // In other words, either this is a name that's not been used before or the
                    // name's not changed
                    if ((location != null) && (location.Id != existingLocationId))
                    {
                        context.Output.WriteLine($"Location {input.name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        inputValid = true;
                    }
                }
            }
            while (!inputValid && !input.cancelled);

            return input;
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
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected Species PromptForSpecies(CommandContext context, int? categoryId, bool expectExisting, string defaultValue, bool isEditing)
        {
            Species species;
            bool inputValid;
            (string name, bool cancelled) input;

            do
            {
                // Reset
                species = null;
                inputValid = false;

                // Read the location name
                input = ReadLineWithDefaultValue(context, "Species name", defaultValue, isEditing);
                if (!input.cancelled)
                {
                    // Clean the string for comparison with existing locations
                    input.name = ToTitleCase(input.name);

                    // See if the species already exists and generate an error
                    // depending on whether or not it should already exist
                    species = context.Factory.Species.Get(s => s.Name == input.name);
                    if ((species == null) && expectExisting)
                    {
                        context.Output.WriteLine($"Species {input.name} does not exist");
                        context.Output.Flush();
                    }
                    else if ((species != null) && !expectExisting)
                    {
                        context.Output.WriteLine($"Species {input.name} already exists");
                        context.Output.Flush();
                    }
                    else
                    {
                        categoryId = species?.CategoryId;
                        inputValid = true;
                    }
                }
            }
            while (!inputValid && !input.cancelled);

            // If the species is NULL, create a new one containing the name
            if (!context.Reader.Cancelled && (species == null))
            {
                species = new Species { Name = input.name, CategoryId = categoryId ?? 0 };
            }

            return species;
        }

        /// <summary>
        /// Prompt for the new location details
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editing"></param>
        /// <returns></returns>
        protected Location PromptForLocation(CommandContext context, Location editing)
        {
            Location location;
            (string value, bool cancelled) input;

            if (editing != null)
            {
                // Editing an existing location so prompt for the name and apply it to the location instance
                input = PromptForLocationNameWhileEditingLocation(context, editing.Name, editing.Id);
                if (input.cancelled) return null;
                location = new Location { Id = editing.Id, Name = input.value };
            }
            else
            {
                // Not editing an existing location so we're adding a new one
                location = PromptForLocationByName(context, null, false);
                if (location == null) return null;
            }

            input = ReadLineWithDefaultValue(context, "Address", editing?.Address, true);
            if (input.cancelled) return null;
            location.Address = input.value;

            input = ReadLineWithDefaultValue(context, "City", editing?.City, true);
            if (input.cancelled) return null;
            location.City = input.value;

            input = ReadLineWithDefaultValue(context, "County", editing?.County, true);
            if (input.cancelled) return null;
            location.County = input.value;

            input = ReadLineWithDefaultValue(context, "Postcode", editing?.Postcode, true);
            if (input.cancelled) return null;
            location.Postcode = input.value;

            input = ReadLineWithDefaultValue(context, "Country", editing?.Country, true);
            if (input.cancelled) return null;
            location.Country = input.value;

            // The "optional"
            (decimal? value, bool cancelled) coordinate = ReadCoordinateWithDefaultValue(context, "Latitude", editing?.Latitude);
            if (input.cancelled) return null;
            location.Latitude = coordinate.value;

            coordinate = ReadCoordinateWithDefaultValue(context, "Longitude", editing?.Longitude);
            if (input.cancelled) return null;
            location.Longitude = coordinate.value;

            return location;
        }

        /// <summary>
        /// Add a sighting
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editing"></param>
        /// <returns></returns>
        protected Sighting PromptForSighting(CommandContext context, Sighting editing)
        {
            // Output a spacer line
            context.Output.WriteLine();
            context.Output.Flush();

            // Empty input is allowed when editing an existing sighting as, in that
            // case, the current sighting properties will be used as the default
            // values for input
            bool isOptionalInput = (editing != null);

            // TODO : Prompt for the location
            string defaultValue = editing?.Location.Name ?? context.CurrentLocation?.Name;
            Location location = PromptForLocationByName(context, defaultValue, true);
            if (context.Reader.Cancelled) return null;

            // Prompt for the date of sighting
            defaultValue = (editing?.Date ?? context.CurrentDate ?? DateTime.Now).ToString(DateFormat);
            DateTime? date = PromptForDate(context, defaultValue, isOptionalInput);
            if (date == null) return null;

            // Prompt for the species
            Species species = PromptForSpecies(context, null, true, editing?.Species.Name, isOptionalInput);
            if (species == null) return null;

            // Prompt for the number of individuals seen
            int? number = PromptForNumber(context, (editing?.Number ?? 0).ToString(), isOptionalInput);
            if (number == null) return null;

            // Yes/No prompt for whether or not young were also seen
            bool withYoung = context.Reader.PromptForYesNo("Seen with young");

            return new Sighting
            {
                Id = editing?.Id ?? 0,
                Location = location,
                LocationId = location.Id,
                Species = species,
                SpeciesId = species.Id,
                Date = date ?? DateTime.Now,
                Number = number ?? 0,
                WithYoung = withYoung
            };
        }
    }
}
