using System;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Interfaces
{
    public interface IImportManager
    {
        event EventHandler<SightingDataExchangeEventArgs> RecordImport;
        void Import(string file);
    }
}