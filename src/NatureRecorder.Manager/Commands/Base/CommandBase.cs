using System;
using System.Collections.Generic;
using System.Globalization;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.BusinessLogic.Extensions;

namespace NatureRecorder.Manager.Commands.Base
{
    public abstract class CommandBase
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        public CommandType Type { get; set; }
        public int MinimumArguments { get; set; }
        public int MaximiumArguments { get; set; }
        public CommandContext RequiredContext { get; set; }

        /// <summary>
        /// Entry point for running the specified command
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public abstract void Run(NatureRecorderFactory _factory, CommandContext context, string[] arguments);

        /// <summary>
        /// Return true if the context is valid
        /// </summary>
        /// <param name="context"></param>
        protected bool ValidForContext(CommandContext context)
        {
            bool valid = (RequiredContext == CommandContext.All) || (context == RequiredContext);
            if (!valid)
            {
                Console.WriteLine($"Command \"{Type}\" is not valid for context \"{context}\"");
            }

            return valid;
        }

        /// <summary>
        /// Return true of the argument count is correct
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected bool ArgumentCountCorrect(string[] arguments)
        {
            bool correct = (arguments.Length >= MinimumArguments) && (arguments.Length <= MaximiumArguments);
            if (!correct)
            {
                Console.WriteLine($"Command \"{Type}\" expects between {MinimumArguments} and {MaximiumArguments} arguments : Received {arguments.Length}");
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
