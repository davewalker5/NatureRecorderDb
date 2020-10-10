using System;
using System.Collections.Generic;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISightingsExportManager
    {
        event EventHandler<SightingDataExchangeEventArgs> RecordExport;
        void Export(IEnumerable<Sighting> sightings, string file);
    }
}