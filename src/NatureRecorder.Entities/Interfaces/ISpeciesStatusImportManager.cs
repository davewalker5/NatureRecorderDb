using System;
using System.Collections.Generic;
using System.IO;
using NatureRecorder.Entities.DataExchange;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISpeciesStatusImportManager
    {
        IList<string> NewSpecies { get; }
        IList<string> NewCategories { get; }
        IList<string> NewSchemes { get; }
        IList<string> NewRatings { get; }

        event EventHandler<SpeciesStatusDataExchangeEventArgs> RecordImport;

        void DetectNewLookups(string file);
        void Import(string file);
        void WriteNewLookupsToStream(StreamWriter output);
    }
}