using System;
using System.Globalization;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class CommandBase
    {
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
                Console.WriteLine($"Command \"{Type}\" is not valid for context \"{context}\"");
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
                Console.WriteLine($"Command \"{Type}\" expects between {MinimumArguments} and {MaximiumArguments} arguments : Received {context.Arguments.Length}");
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
    }
}
