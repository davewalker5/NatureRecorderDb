using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class LocationManager : ILocationManager
    {
        private readonly NatureRecorderDbContext _context;
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        internal LocationManager(NatureRecorderDbContext context)
        {
            _context = context;
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
            List<Location> locations = await _context.Locations
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
                results = _context.Locations;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _context.Locations
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
                results = _context.Locations;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Locations
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
                _context.Locations.Add(location);
                _context.SaveChanges();
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
                await _context.Locations.AddAsync(location);
                await _context.SaveChangesAsync();
            }

            return location;
        }
    }
}
