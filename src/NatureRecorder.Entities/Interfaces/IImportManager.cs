﻿using System;
using System.Collections.Generic;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Interfaces
{
    public interface IImportManager
    {
        event EventHandler<SightingDataExchangeEventArgs> RecordImport;

        IList<string> NewLocations { get; }
        IList<string> NewSpecies { get; }
        IList<string> NewCategories { get; }

        void Import(string file);
        void DetectNewLookups(string file);
        void WriteNewLookupsToConsole();
    }
}
