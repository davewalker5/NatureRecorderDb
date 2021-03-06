﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NatureRecorder.Interpreter.Interfaces;

namespace NatureRecorder.Interpreter.Logic
{
    [ExcludeFromCodeCoverage]
    public class ConsoleCommandReader : IInputReader
    {
        public bool Cancelled { get; private set; }

        /// <summary>
        /// Read a line from the console with ENTER to accept the input and
        /// ESC to cancel it
        /// </summary>
        /// <returns></returns>
        public string ReadLine(string prompt, bool allowBlank)
        {
            StringBuilder builder = new StringBuilder();
            bool invalid = false;
            bool completed;
            string input;

            do
            {
                Console.Write($"{prompt} [ESC to cancel]: ");

                // Reset for the first or previous (invalid) attempt
                builder.Clear();
                completed = false;

                do
                {
                    // Read the next keypress and see what it is
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            completed = true;
                            break;
                        case ConsoleKey.Escape:
                            Console.Write("<< Cancelled >>");
                            Cancelled = true;
                            break;
                        case ConsoleKey.Backspace:
                            if (builder.Length > 0)
                            {
                                // Back up by one character, wipe the last character
                                // with a space and back up again
                                Console.Write("\b \b");
                                builder.Remove(builder.Length - 1, 1);
                            }
                            break;
                        default:
                            // Just write the character to the console and add it
                            // to the string builder provided the ALT and CTRL modifiers
                            // aren't pressed
                            bool ctrlIsPressed = (key.Modifiers & ConsoleModifiers.Control) > 0;
                            bool altIsPressed = (key.Modifiers & ConsoleModifiers.Alt) > 0;
                            if (!ctrlIsPressed && !altIsPressed)
                            {
                                Console.Write(key.KeyChar);
                                builder.Append(key.KeyChar);
                            }
                            break;
                    }
                }
                while (!completed && !Cancelled);

                // Move on to the next line of the console
                Console.WriteLine();

                // The string is invalid if it's blank, we're not allowing blanks
                // and input hasn't been cancelled
                if (!Cancelled)
                {
                    input = builder.ToString().Trim();
                    invalid = (input.Length == 0) && !allowBlank;
                    if (invalid)
                    {
                        Console.WriteLine($"\"{input}\" is not valid input");
                    }
                }
            }
            while (invalid);

            // If the input was completed rather than cancelled, return it.
            // Otherwise, return NULL
            return (!Cancelled) ? builder.ToString() : null;
        }

        /// <summary>
        /// Read and return input of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public T Read<T>(string prompt)
        {
            Type type = typeof(T);
            T result = default;
            bool invalid = false;

            // Blanks will be interpreted as NULLs so we only allow blank input
            // if the underlying type is NULLable
            bool allowBlank = Nullable.GetUnderlyingType(type) != null;

            do
            {
                // Read the input
                string input = ReadLine(prompt, allowBlank);

                if (!Cancelled)
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        // Input's blank. This is only allowed if the type is nullable
                        // in which case there's no need to attempt to parse the value
                        // as the assignment of "default", above, will have set the value
                        // to NULL.
                        invalid = !allowBlank;
                    }
                    else
                    {
                        try
                        {
                            // Attempt to convert the string we've received to the specified
                            // type
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            result = (T)converter.ConvertFromString(input);
                            invalid = false;
                        }
                        catch
                        {
                            Console.WriteLine($"\"{input}\" is not valid input");
                            invalid = true;
                        }
                    }
                }
            }
            while (!Cancelled && invalid);

            return result;
        }

        /// <summary>
        /// Prompt for one of a set of single character options
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="options"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="defaultOption"></param>
        /// <returns></returns>
        public char? PromptForOption(string prompt, IEnumerable<char> options, char? defaultOption)
        {
            char? selected = null;

            // Convert the options to uppercase and make sure they're distinct
            IEnumerable<char> distinctOptions = options.Select(o => Char.ToUpper(o)).Distinct();
            string optionList = string.Join(",", distinctOptions);

            // Construct the prompt
            string promptWithDefault = (defaultOption != null) ?
                                            $"{prompt} ({optionList}) [{defaultOption}]" :
                                            $"{prompt} ({optionList})";

            do
            {
                // Read the input, allowing blank input if the default option has been specified
                string input = ReadLine(promptWithDefault, (defaultOption != null));
                if (!Cancelled)
                {
                    input = input.Trim().ToUpper();
                    if ((defaultOption != null) && string.IsNullOrEmpty(input))
                    {
                        selected = defaultOption;
                    }
                    else if ((input.Length == 1) && distinctOptions.Contains(input[0]))
                    {
                        selected = input[0];
                    }
                    else
                    {
                        Console.WriteLine($"'{input}' is not a valid input");
                    }
                }
            }
            while (!Cancelled && (selected == null));

            return selected;
        }

        /// <summary>
        /// Prompt for a y/Y or n/N keypress
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool PromptForYesNo(string prompt, char? defaultValue)
        {
            char? selected = PromptForOption(prompt, new char[] { 'Y', 'N' }, defaultValue);
            return (selected == 'Y');
        }
    }
}
