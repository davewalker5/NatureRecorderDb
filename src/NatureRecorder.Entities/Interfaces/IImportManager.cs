using System;
using System.Collections.Generic;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Interfaces
{
    public interface IImportManager
    {
        event EventHandler<SightingDataExchangeEventArgs> RecordImport;

        IEnumerable<string> NewLocations { get; }
        IEnumerable<string> NewSpecies { get; }
        IEnumerable<string> NewCategories { get; }

        void Import(string file);
        void DetectNewLookups(string file);
        void WriteNewLookupsToConsole();
    }
}
