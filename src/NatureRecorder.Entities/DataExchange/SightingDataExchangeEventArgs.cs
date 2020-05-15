using System;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class SightingDataExchangeEventArgs : EventArgs
    {
        public long RecordCount { get; set; }
        public Sighting Sighting { get; set; }
    }
}
