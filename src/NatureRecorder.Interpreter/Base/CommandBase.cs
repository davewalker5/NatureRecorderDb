using System;
using System.Globalization;
using System.IO;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class CommandBase
    {
        private const string DateFormat = "yyyy-MM-dd";
        protected const string Wildcard = "*";

        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        public CommandType Type { get; set; }
        public int MinimumArguments { get; set; }
        public int MaximiumArguments { get; set; }
        public CommandMode RequiredMode { get; set; }

        /// <summary>
        /// Entry point for running the specified command
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public abstract void Run(CommandContext context);

        /// <summary>
        /// Return true if the current command mode is valid
        /// </summary>
        /// <param name="context"></param>
        protected bool ValidForCommandMode(CommandContext context)
        {
            bool valid = (RequiredMode == CommandMode.All) || (context.Mode == RequiredMode);
            if (!valid)
            {
                context.Output.WriteLine($"Command \"{Type}\" is not valid for command mode \"{context.Mode}\"");
                context.Output.Flush();
            }

            return valid;
        }

        /// <summary>
        /// Return true of the argument count is correct
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool ArgumentCountCorrect(CommandContext context)
        {
            bool correct = (context.Arguments.Length >= MinimumArguments) && (context.Arguments.Length <= MaximiumArguments);
            if (!correct)
            {
                context.Output.WriteLine($"Command \"{Type}\" expects between {MinimumArguments} and {MaximiumArguments} arguments : Received {context.Arguments.Length}");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Clean up the specified string and return it in title case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ToTitleCase(string value)
        {
            return _textInfo.ToTitleCase(value.CleanString());
        }

        /// <summary>
        /// Identify the species from the name given in the arguments at the
        /// specified index. A "wildcard" name causes a null return
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int? GetSpecies(CommandContext context, int index)
        {
            int? speciesId = null;

            string speciesName = context.CleanArgument(index);
            if ((speciesName != null) && (speciesName != Wildcard))
            {
                Species species = context.Factory
                                         .Species
                                         .Get(s => s.Name == speciesName);
                if (species == null)
                {
                    string message = $"Species '{speciesName}' does not exist";
                    throw new SpeciesDoesNotExistException(message);
                }

                speciesId = species.Id;
            }

            return speciesId;
        }

        /// <summary>
        /// Identify the category from the name given in the arguments at the
        /// specified index. A "wildcard" name causes a null return
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int? GetCategory(CommandContext context, int index)
        {
            int? categoryId = null;

            string categoryName = context.CleanArgument(index);
            if ((categoryName != null) && (categoryName != Wildcard))
            {
                Category category = context.Factory
                                           .Categories
                                           .Get(c => c.Name == categoryName);
                if (category == null)
                {
                    string message = $"Category '{categoryName}' does not exist";
                    throw new CategoryDoesNotExistException(message);
                }

                categoryId = category.Id;
            }

            return categoryId;
        }

        /// <summary>
        /// Identify the location from the name given in the arguments at the
        /// specified index
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int? GetLocation(CommandContext context, int index)
        {
            int? locationId = null;

            string locationName = context.CleanArgument(index);
            if ((locationName != null) && (locationName != Wildcard))
            {
                Location location = context.Factory
                                            .Locations
                                            .Get(l => l.Name == locationName);
                if (location == null)
                {
                    string message = $"Location '{locationName}' does not exist";
                    throw new LocationDoesNotExistException(message);
                }

                locationId = location.Id;
            }

            return locationId;
        }

        /// <summary>
        /// Return the date/time represented by the input string or NULL if the format
        /// isn't as expected
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected DateTime? GetDateFromArgument(string argument, StreamWriter output)
        {
            DateTime? result = null;
            DateTime parsed;

            if (DateTime.TryParseExact(argument, DateFormat, null, DateTimeStyles.None, out parsed))
            {
                result = parsed;
            }
            else
            {
                output.WriteLine($"\"{argument}\" is not in the expected format ({DateFormat})");
            }

            return result;
        }

        /// <summary>
        /// Get a date from the argument at the specified index, defaulting to
        /// the specified default value if the index is out of range
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="index"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected DateTime? GetDateFromArgument(string[] arguments, int index, DateTime? defaultDate, StreamWriter output)
        {
            DateTime? date = defaultDate;
            if ((index >= 0) && (index < arguments.Length))
            {
                date = GetDateFromArgument(arguments[index], output);
            }

            return date;
        }
    }
}
