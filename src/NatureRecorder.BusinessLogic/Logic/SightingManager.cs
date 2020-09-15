using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Interfaces;
using NatureRecorder.Entities.Reporting;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class SightingManager : ISightingManager
    {
        private NatureRecorderFactory _factory;

        internal SightingManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first sighting matching the specified criteria along with the associated entites
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Sighting Get(Expression<Func<Sighting, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Get the first sighting matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate)
        {
            List<Sighting> sightings = await _factory.Context.Sightings
                                                             .Where(predicate)
                                                             .ToListAsync();
            return sightings.FirstOrDefault();
        }

        /// <summary>
        /// Get the sightings matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Sighting> sightings;

            if (predicate == null)
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Species)
                                            .ThenInclude(sp => sp.Category)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize);
            }
            else
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Species)
                                            .ThenInclude(sp => sp.Category)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .Where(predicate);
            }

            return sightings;
        }

        /// <summary>
        /// Get the sightings matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings;

            if (predicate == null)
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Species)
                                            .ThenInclude(sp => sp.Category)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsAsyncEnumerable();
            }
            else
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Species)
                                            .ThenInclude(sp => sp.Category)
                                            .Where(predicate)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsAsyncEnumerable();
            }

            return sightings;
        }

        /// <summary>
        /// Add a new sighting
        /// </summary>
        /// <param name="number"></param>
        /// <param name="withYoung"></param>
        /// <param name="date"></param>
        /// <param name="locationId"></param>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        public Sighting Add(int number, bool withYoung, DateTime date, int locationId, int speciesId)
        {
            Sighting sighting = new Sighting
            {
                Number = number,
                WithYoung = withYoung,
                Date = date,
                LocationId = locationId,
                SpeciesId = speciesId
            };

            _factory.Context.Sightings.Add(sighting);
            _factory.Context.SaveChanges();

            _factory.Context.Entry(sighting).Reference(s => s.Location).Load();
            _factory.Context.Entry(sighting).Reference(s => s.Species).Load();
            _factory.Context.Entry(sighting.Species).Reference(f => f.Category).Load();

            return sighting;
        }

        /// <summary>
        /// Add a new sighting based on the supplied template
        /// </summary>
        /// <param name="newSighting"></param>
        /// <returns></returns>
        public Sighting Add(Sighting template)
        {
            // Retrieve/create the location, category and species. The logic to
            // create new records or return existing ones is in these methods
            Location location = _factory.Locations.Add(template.Location.Name,
                                                       template.Location.Address,
                                                       template.Location.City,
                                                       template.Location.County,
                                                       template.Location.Postcode,
                                                       template.Location.Country,
                                                       template.Location.Latitude,
                                                       template.Location.Longitude);
            Species species = _factory.Species.Add(template.Species.Name, template.Species.Category.Name);

            // Add a new sighting
            return Add(template.Number, template.WithYoung, template.Date, location.Id, species.Id);
        }

        /// <summary>
        /// Add a new sighting
        /// </summary>
        /// <param name="number"></param>
        /// <param name="withYoung"></param>
        /// <param name="date"></param>
        /// <param name="locationId"></param>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(int number, bool withYoung, DateTime date, int locationId, int speciesId)
        {
            Sighting sighting = new Sighting
            {
                Number = number,
                WithYoung = withYoung,
                Date = date,
                LocationId = locationId,
                SpeciesId = speciesId
            };

            await _factory.Context.Sightings.AddAsync(sighting);
            await _factory.Context.SaveChangesAsync();

            await _factory.Context.Entry(sighting).Reference(s => s.Location).LoadAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Species).LoadAsync();
            await _factory.Context.Entry(sighting.Species).Reference(f => f.Category).LoadAsync();

            return sighting;
        }

        /// <summary>
        /// Return a list of sightings at a specified location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByLocation(int locationId, int pageNumber, int pageSize)
                => List(s => s.LocationId == locationId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings at a specified location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListByLocationAsync(int locationId, int pageNumber, int pageSize)
            => ListAsync(s => s.LocationId == locationId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings for a specified category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByCategory(int categoryId, int pageNumber, int pageSize)
                => List(s => s.Species.CategoryId == categoryId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings for a specified category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListByCategoryAsync(int categoryId, int pageNumber, int pageSize)
            => ListAsync(s => s.Species.CategoryId == categoryId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings for a specified species
        /// </summary>
        /// <param name="speciesId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListBySpecies(int speciesId, int pageNumber, int pageSize)
                => List(s => s.SpeciesId == speciesId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings for a specified species
        /// </summary>
        /// <param name="speciesId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListBySpeciesAsync(int speciesId, int pageNumber, int pageSize)
            => ListAsync(s => s.SpeciesId == speciesId, pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings between two dates
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByDate(DateTime from, DateTime to, int pageNumber, int pageSize)
                => List(s => (s.Date >= from) && (s.Date <= to), pageNumber, pageSize);

        /// <summary>
        /// Return a list of sightings between two dates
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListByDateAsync(DateTime from, DateTime to, int pageNumber, int pageSize)
                => ListAsync(s => (s.Date >= from) && (s.Date <= to), pageNumber, pageSize);

        /// <summary>
        /// Generate a sightings summary for the specified report filters
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="locationId"></param>
        /// <param name="categoryId"></param>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        public Summary Summarise(DateTime from, DateTime to, int? locationId, int? categoryId, int? speciesId)
        {
            IEnumerable<Sighting> sightings = ListByDate(from, to, 1, int.MaxValue);

            if (locationId != null)
            {
                sightings = sightings.Where(s => s.LocationId == locationId);
            }

            if (categoryId != null)
            {
                sightings = sightings.Where(s => s.Species.CategoryId == categoryId);
            }

            if (speciesId != null)
            {
                sightings = sightings.Where(s => s.SpeciesId == speciesId);
            }

            return new Summary { Sightings = sightings };
        }

        /// <summary>
        /// Generate a sightings summary for the specified report filters
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="locationId"></param>
        /// <param name="categoryId"></param>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        public async Task<Summary> SummariseAsync(DateTime from, DateTime to, int? locationId, int? categoryId, int? speciesId)
        {
            IEnumerable<Sighting> sightings = await ListByDateAsync(from, to, 1, int.MaxValue).ToListAsync();

            if (locationId != null)
            {
                sightings = sightings.Where(s => s.LocationId == locationId);
            }

            if (categoryId != null)
            {
                sightings = sightings.Where(s => s.Species.CategoryId == categoryId);
            }

            if (speciesId != null)
            {
                sightings = sightings.Where(s => s.SpeciesId == speciesId);
            }

            return new Summary { Sightings = sightings };
        }

        /// <summary>
        /// Delete the sighting with the specified Id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            // Get the sighting and make sure it exists
            Sighting sighting = _factory.Context
                                        .Sightings
                                        .First(s => s.Id == id);

            if (sighting == null)
            {
                string message = $"Sighting '{id}' does not exist";
                throw new SightingDoesNotExistException(message);
            }

            // Delete the sighting
            _factory.Context.Sightings.Remove(sighting);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the sighting with the specified Id
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(int id)
        {
            // Get the sighting and make sure it exists
            Sighting sighting = await _factory.Context
                                              .Sightings
                                              .AsAsyncEnumerable()
                                              .FirstAsync(s => s.Id == id);

            if (sighting == null)
            {
                string message = $"Sighting '{id}' does not exist";
                throw new SightingDoesNotExistException(message);
            }

            // Delete the sighting
            _factory.Context.Sightings.Remove(sighting);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
