using System;
using System.Collections.Generic;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISpeciesStatusExportManager
    {
        event EventHandler<SpeciesStatusDataExchangeEventArgs> RecordExport;

        void Export(IEnumerable<SpeciesStatusRating> ratings, string file);
    }
}