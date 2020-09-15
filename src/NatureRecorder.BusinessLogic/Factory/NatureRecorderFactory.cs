using System;
using NatureRecorder.BusinessLogic.Logic;
using NatureRecorder.Data;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Factory
{
    public class NatureRecorderFactory
    {
        private readonly Lazy<ILocationManager> _locations = null;
        private readonly Lazy<ICategoryManager> _categories = null;
        private readonly Lazy<ISpeciesManager> _species = null;
        private readonly Lazy<ISightingManager> _sightings = null;
        private readonly Lazy<IUserManager> _users = null;
        private readonly Lazy<IExportManager> _export = null;
        private readonly Lazy<IImportManager> _import = null;

        public NatureRecorderDbContext Context { get; private set; }
        public ILocationManager Locations { get { return _locations.Value; } }
        public ICategoryManager Categories { get { return _categories.Value; } }
        public ISpeciesManager Species { get { return _species.Value; } }
        public ISightingManager Sightings { get { return _sightings.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public IExportManager Export { get { return _export.Value; } }
        public IImportManager Import { get { return _import.Value; } }

        public NatureRecorderFactory(NatureRecorderDbContext context)
        {
            Context = context;
            _locations = new Lazy<ILocationManager>(() => new LocationManager(this));
            _categories = new Lazy<ICategoryManager>(() => new CategoryManager(this));
            _species = new Lazy<ISpeciesManager>(() => new SpeciesManager(this));
            _sightings = new Lazy<ISightingManager>(() => new SightingManager(this));
            _users = new Lazy<IUserManager>(() => new UserManager(context));
            _export = new Lazy<IExportManager>(() => new ExportManager());
            _import = new Lazy<IImportManager>(() => new ImportManager(this));
        }
    }
}
