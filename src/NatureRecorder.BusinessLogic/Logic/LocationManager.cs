using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class LocationManager : ILocationManager
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;
        private NatureRecorderFactory _factory;

        internal LocationManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Location Get(Expression<Func<Location, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Location> GetAsync(Expression<Func<Location, bool>> predicate)
        {
            List<Location> locations = await _factory.Context.Locations
                                                     .Where(predicate)
                                                     .ToListAsync();
            return locations.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<Location> List(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Location> results;
            if (predicate == null)
            {
                results = _factory.Context.Locations;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _factory.Context.Locations
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize);
            }

            return results;
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Location> results;
            if (predicate == null)
            {
                results = _factory.Context.Locations;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Locations
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <param name="gps"></param>
        /// <returns></returns>
        public Location Add(string name,
                            string address,
                            string city,
                            string county,
                            string postcode,
                            string country,
                            decimal? latitude,
                            decimal? longitude)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            Location location = Get(a => a.Name == name);

            if (location == null)
            {
                location = new Location
                {
                    Name = name,
                    Address = address,
                    City = city,
                    County = county,
                    Postcode = postcode,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude
                };
                _factory.Context.Locations.Add(location);
                _factory.Context.SaveChanges();
            }

            return location;
        }

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<Location> AddAsync(string name,
                                             string address,
                                             string city,
                                             string county,
                                             string postcode,
                                             string country,
                                             decimal? latitude,
                                             decimal? longitude)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            Location location = await GetAsync(a => a.Name == name);

            if (location == null)
            {
                location = new Location
                {
                    Name = name,
                    Address = address,
                    City = city,
                    County = county,
                    Postcode = postcode,
                    Country = country,
                    Latitude = latitude,
                    Longitude = longitude
                };
                await _factory.Context.Locations.AddAsync(location);
                await _factory.Context.SaveChangesAsync();
            }

            return location;
        }

        /// <summary>
        /// Delete the location with the specified name
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            // Get the location and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Location location = Get(l => l.Name == name);

            if (location == null)
            {
                string message = $"Location '{name}' does not exist";
                throw new LocationDoesNotExistException(message);
            }

            // Check the location isn't used in any sightings
            Sighting sighting = _factory.Context
                                        .Sightings
                                        .FirstOrDefault(s => s.LocationId == location.Id);

            if (sighting != null)
            {
                string message = $"Cannot delete location '{name}' while it is referenced in sightings";
                throw new LocationIsInUseException(message);
            }

            // Delete the location
            _factory.Context.Locations.Remove(location);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the location with the specified name
        /// </summary>
        /// <param name="name"></param>
        public async Task DeleteAsync(string name)
        {
            // Get the location and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Location location = await GetAsync(l => l.Name == name);

            if (location == null)
            {
                string message = $"Location '{name}' does not exist";
                throw new LocationDoesNotExistException(message);
            }

            // Check the location isn't used in any sightings
            Sighting sighting = await _factory.Context
                                              .Sightings
                                              .AsAsyncEnumerable()
                                              .FirstOrDefaultAsync(s => s.LocationId == location.Id);

            if (sighting != null)
            {
                string message = $"Cannot delete location '{name}' while it is referenced in sightings";
                throw new LocationIsInUseException(message);
            }

            // Delete the location
            _factory.Context.Locations.Remove(location);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
