using System.Collections.Generic;
using System.Linq;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Reporting
{
    public class Summary
    {
        public IEnumerable<Location> Locations { get { return Sightings.Select(s => s.Location).Distinct(); } }
        public IEnumerable<Category> Categories { get { return Sightings.Select(s => s.Species.Category).Distinct(); } }
        public IEnumerable<Species> Species { get { return Sightings.Select(s => s.Species).Distinct(); } }
        public IEnumerable<Sighting> Sightings { get; set; }
    }
}
