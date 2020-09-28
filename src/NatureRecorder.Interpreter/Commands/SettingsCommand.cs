using System;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class SettingsCommand : CommandBase
    {
        public SettingsCommand()
        {
            Type = CommandType.settings;
            MinimumArguments = 1;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.Interactive;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument is the setting type or action, that determines the
                // required argument count
                SettingType type = GetSettingType(context);
                if (ArgumentCountCorrectForSettingType(context, type))
                {
                    switch (type)
                    {
                        case SettingType.Location:
                            SetDefaultLocation(context);
                            break;
                        case SettingType.List:
                            context.Settings.List(context.Output);
                            break;
                        case SettingType.Clear:
                            context.Settings.Clear();
                            context.Settings.Save();
                            context.Settings.List(context.Output);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Check the argument count is correct for the specified report type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool ArgumentCountCorrectForSettingType(CommandContext context, SettingType type)
        {
            (int minimum, int maximum) counts = GetRequiredArgumentCounts(type);
            bool correct = ((context.Arguments.Length >= counts.minimum) && (context.Arguments.Length <= counts.maximum));
            if (!correct)
            {
                context.Output.WriteLine($"Incorrect argument count for the \"{Type} {context.Arguments[0]}\" command");
                context.Output.Flush();
            }

            return correct;
        }

        /// <summary>
        /// Set the default location
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void SetDefaultLocation(CommandContext context)
        {
            // Confirm the location exists
            string locationName = context.CleanArgument(1);
            Location location = context.Factory.Locations
                                               .Get(l => l.Name == locationName);
            if (location != null)
            {
                // It does, so update and save the settings
                context.Settings.Location = locationName;
                context.Settings.Save();
                context.Output.WriteLine($"Default location set to '{locationName}'");
                context.Output.Flush();
            }
            else
            {
                string message = $"Location '{locationName}' does not exist";
                throw new LocationDoesNotExistException(message);
            }
        }

        /// <summary>
        /// Return the argument limits based on the setting type/action
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private (int minimum, int maximum) GetRequiredArgumentCounts(SettingType type)
        {
            (int minimum, int maximum) counts;

            switch (type)
            {
                case SettingType.List:
                    counts.minimum = 1;
                    counts.maximum = 1;
                    break;
                case SettingType.Clear:
                    counts.minimum = 1;
                    counts.maximum = 1;
                    break;
                default:
                    counts.minimum = 2;
                    counts.maximum = 2;
                    break;
            }

            return counts;
        }

        /// <summary>
        /// Determine the setting type/action
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private SettingType GetSettingType(CommandContext context)
        {
            string settingTypeName = context.CleanArgument(0);
            if (!Enum.TryParse<SettingType>(settingTypeName, out SettingType type))
            {
                string message = $"Unknown setting type or action '{settingTypeName}'";
                throw new UnknownSettingTypeException(message);
            }

            return type;
        }
    }
}
