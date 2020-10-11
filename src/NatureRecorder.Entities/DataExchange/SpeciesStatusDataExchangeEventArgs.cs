using System;
using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class SpeciesStatusDataExchangeEventArgs : EventArgs
    {
        public long RecordCount { get; set; }
        public SpeciesStatusRating Rating { get; set; }
    }
}
