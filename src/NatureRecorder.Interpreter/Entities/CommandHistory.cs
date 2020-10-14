using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using NatureRecorder.Entities.Exceptions;
using static System.Environment;

namespace NatureRecorder.Interpreter.Entities
{
    public class CommandHistory
    {
        private const string HistoryFileName = "naturerecorder.history";
        private static string _defaultLocation = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), HistoryFileName);

        private List<string> _history;

        public int Count { get { return _history?.Count ?? 0;  } }
        public string HistoryFile { get; set; }

        /// <summary>
        /// Default constructor - the history file is in the user's profile
        /// </summary>
        [ExcludeFromCodeCoverage]
        public CommandHistory() : this(_defaultLocation)
        {
        }

        /// <summary>
        /// Construct the instance using the specified location for the history
        /// file
        /// </summary>
        /// <param name="location"></param>
        public CommandHistory(string location)
        {
            HistoryFile = location;
            Read();
        }

        /// <summary>
        /// Return the numbered entry from the history
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string Get(int entry)
        {
            // History entries are counted from 1, not 0
            if ((Count == 0) || (entry < 1) || (entry > Count))
            {
                string message = $"{entry} is out of range";
                throw new InvalidHistoryEntryException(message);
            }

            return _history[entry - 1];
        }

        /// <summary>
        /// Append a command to the history
        /// </summary>
        /// <param name="command"></param>
        public void Add(string command)
        {
            _history.Add(command);

            using (StreamWriter writer = new StreamWriter(HistoryFile, true))
            {
                writer.WriteLine(command);
            }
        }

        /// <summary>
        /// Show where the history file is located
        /// </summary>
        /// <param name="writer"></param>
        public void Location(StreamWriter writer)
        {
            writer.WriteLine(HistoryFile);
            writer.Flush();
        }

        /// <summary>
        /// List the history to the specified stream
        /// </summary>
        /// <param name="writer"></param>
        public void List(StreamWriter writer)
        {
            if (Count == 0)
            {
                writer.WriteLine("History is empty");
            }
            else
            {
                for (int i = 0; i < _history.Count(); i++)
                {
                    string number = (i + 1).ToString().PadLeft(6);
                    writer.WriteLine($"{number} {_history[i]}");
                }
            }

            writer.Flush();
        }

        /// <summary>
        /// Clear the command history
        /// </summary>
        public void Clear()
        {
            _history.Clear();

            if (File.Exists(HistoryFile))
            {
                File.Delete(HistoryFile);
            }
        }

        /// <summary>
        /// Read the command history from the default history file
        /// </summary>
        private void Read()
        {
            if (File.Exists(HistoryFile))
            {
                _history = File.ReadAllLines(HistoryFile).ToList();
            }
            else
            {
                _history = new List<string>();
            }
        }
    }
}
