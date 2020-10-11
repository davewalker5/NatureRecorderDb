using System;
using NatureRecorder.Entities.DataExchange;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Base
{
    public abstract class DataExchangeCommandBase : CommandBase
    {
        /// <summary>
        /// Determine the required data exchange type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected DataExchangeType GetDataExchangeType(CommandContext context)
        {
            string exportTypeName = context.CleanArgument(0);
            if (!Enum.TryParse<DataExchangeType>(exportTypeName, out DataExchangeType type))
            {
                string message = "Cannot generate unknown export type";
                throw new UnknownDataExchangeTypeException(message);
            }

            return type;
        }

        /// <summary>
        /// Callback handler for record sighting import/export events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSightingRecordImportExport(object sender, SightingDataExchangeEventArgs e)
        {
            Console.WriteLine($"{e.RecordCount} : {e.Sighting.Date.ToShortDateString()} {e.Sighting.Species.Name} {e.Sighting.Location.Name}");
        }

        /// <summary>
        /// Callback handler for conservation status record import/export events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSpeciesStatusRecordImportExport(object sender, SpeciesStatusDataExchangeEventArgs e)
        {
            Console.WriteLine($"{e.RecordCount} : {e.Rating.Species.Name} rated {e.Rating.Rating.Name} on scheme {e.Rating.Rating.Scheme.Name}");
        }
    }
}
