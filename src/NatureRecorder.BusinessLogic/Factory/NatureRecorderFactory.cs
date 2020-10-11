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
        private readonly Lazy<ISightingsExportManager> _export = null;
        private readonly Lazy<ISightingsImportManager> _import = null;
        private readonly Lazy<IStatusSchemeSchemeManager> _statusSchemes = null;
        private readonly Lazy<IStatusRatingManager> _statusRatings = null;
        private readonly Lazy<ISpeciesStatusRatingManager> _speciesStatusRatings = null;
        private readonly Lazy<ISpeciesStatusExportManager> _speciesStatusExport = null;
        private readonly Lazy<ISpeciesStatusImportManager> _speciesStatusImport = null;

        public NatureRecorderDbContext Context { get; private set; }
        public ILocationManager Locations { get { return _locations.Value; } }
        public ICategoryManager Categories { get { return _categories.Value; } }
        public ISpeciesManager Species { get { return _species.Value; } }
        public ISightingManager Sightings { get { return _sightings.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public ISightingsExportManager SightingsExport { get { return _export.Value; } }
        public ISightingsImportManager SightingsImport { get { return _import.Value; } }
        public IStatusSchemeSchemeManager StatusSchemes { get { return _statusSchemes.Value; } }
        public IStatusRatingManager StatusRatings { get { return _statusRatings.Value; } }
        public ISpeciesStatusRatingManager SpeciesStatusRatings { get { return _speciesStatusRatings.Value; } }
        public ISpeciesStatusExportManager SpeciesStatusExport { get { return _speciesStatusExport.Value; } }
        public ISpeciesStatusImportManager SpeciesStatusImport { get { return _speciesStatusImport.Value; } }

        public NatureRecorderFactory(NatureRecorderDbContext context)
        {
            Context = context;
            _locations = new Lazy<ILocationManager>(() => new LocationManager(this));
            _categories = new Lazy<ICategoryManager>(() => new CategoryManager(this));
            _species = new Lazy<ISpeciesManager>(() => new SpeciesManager(this));
            _sightings = new Lazy<ISightingManager>(() => new SightingManager(this));
            _users = new Lazy<IUserManager>(() => new UserManager(context));
            _export = new Lazy<ISightingsExportManager>(() => new SightingsExportManager());
            _import = new Lazy<ISightingsImportManager>(() => new SightingsImportManager(this));
            _statusSchemes = new Lazy<IStatusSchemeSchemeManager>(() => new StatusSchemeSchemeManager(this));
            _statusRatings = new Lazy<IStatusRatingManager>(() => new StatusRatingManager(this));
            _speciesStatusRatings = new Lazy<ISpeciesStatusRatingManager>(() => new SpeciesStatusRatingManager(this));
            _speciesStatusExport = new Lazy<ISpeciesStatusExportManager>(() => new SpeciesStatusExportManager());
            _speciesStatusImport = new Lazy<ISpeciesStatusImportManager>(() => new SpeciesStatusImportManager(this));
        }
    }
}
