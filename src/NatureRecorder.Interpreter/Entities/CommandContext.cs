﻿using System;
using System.Globalization;
using System.IO;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Interpreter.Interfaces;

namespace NatureRecorder.Interpreter.Entities
{
    public class CommandContext
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        public NatureRecorderFactory Factory { get; set; }
        public CommandMode Mode { get; set; }
        public string[] Arguments { get; set; }
        public IInputReader Reader { get; set; }
        public StreamWriter Output { get; set; }
        public Location CurrentLocation { get; set; }
        public DateTime? CurrentDate { get; set; }

        /// <summary>
        /// Return a cleaned-up version of the specified argument converted to
        /// title case
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string CleanArgument(int index)
        {
            return _textInfo.ToTitleCase(Arguments[index].CleanString());
        }
    }
}
