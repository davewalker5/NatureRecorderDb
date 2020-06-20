using System;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class DataExchangeCommandBase : CommandBase
    {
        /// <summary>
        /// Callback handler for record import/export events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnRecordImportExport(object sender, SightingDataExchangeEventArgs e)
        {
            Console.WriteLine($"{e.RecordCount} : {e.Sighting.Date.ToShortDateString()} {e.Sighting.Species.Name} {e.Sighting.Location.Name}");
        }
    }
}
