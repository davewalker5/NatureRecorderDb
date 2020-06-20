using System;
using System.ComponentModel;
using System.IO;
using NatureRecorder.Interpreter.Interfaces;

namespace NatureRecorder.Interpreter.Logic
{
    public class StreamCommandReader : IInputReader
    {
        private readonly StreamReader _input;

        public bool Cancelled { get; private set; }

        public StreamCommandReader(StreamReader reader)
        {
            _input = reader;
        }

        /// <summary>
        /// Read a line of input from the stream. Prompting is assumed not to be
        /// necessary in this context
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="allowBlank"></param>
        /// <returns></returns>
        public string ReadLine(string prompt, bool allowBlank)
        {
            string input = null;

            Cancelled = _input.EndOfStream;
            if (!Cancelled)
            {
                input = _input.ReadLine();
            }

            return input;
        }


        /// <summary>
        /// Read and return input of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public T Read<T>(string prompt)
        {
            T result = default;

            Cancelled = _input.EndOfStream;
            if (!Cancelled)
            {
                string input = _input.ReadLine();
                Type type = typeof(T);
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                result = (T)converter.ConvertFromString(input);
            }

            return result;
        }

        /// <summary>
        /// Read the next line and return true if it's "y" or "Y" and false
        /// otherwise
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public bool PromptForYesNo(string prompt)
        {
            bool result = false;

            Cancelled = _input.EndOfStream;
            if (!Cancelled)
            {
                string input = _input.ReadLine().Trim();
                result = input.Equals("y", StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}
