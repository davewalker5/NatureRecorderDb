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

        private List<string> _history;
        private readonly string _historyFile;

        public int Count { get { return _history?.Count ?? 0;  } }

        public CommandHistory()
        {
            // Get the history file path and, if it exists, load the history from it
            _historyFile = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), HistoryFileName);
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

            using (StreamWriter writer = new StreamWriter(_historyFile, true))
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
            writer.WriteLine(_historyFile);
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

            if (File.Exists(_historyFile))
            {
                File.Delete(_historyFile);
            }
        }

        /// <summary>
        /// Read the command history from the default history file
        /// </summary>
        private void Read()
        {
            if (File.Exists(_historyFile))
            {
                _history = File.ReadAllLines(_historyFile).ToList();
            }
            else
            {
                _history = new List<string>();
            }
        }
    }
}
