﻿using System.IO;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Interpreter.Interfaces;

namespace NatureRecorder.Interpreter.Entities
{
    public class CommandContext
    {
        public NatureRecorderFactory Factory { get; set; }
        public CommandMode Mode { get; set; }
        public string[] Arguments { get; set; }
        public IInputReader Reader { get; set; }
        public StreamWriter Output { get; set; }
    }
}