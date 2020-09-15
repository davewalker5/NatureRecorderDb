using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ILocationManager
    {
        Location Add(string name, string address, string city, string county, string postcode, string country, decimal? latitude, decimal? longitude);
        Task<Location> AddAsync(string name, string address, string city, string county, string postcode, string country, decimal? latitude, decimal? longitude);
        void Delete(string name);
        Task DeleteAsync(string name);
        Location Get(Expression<Func<Location, bool>> predicate);
        Task<Location> GetAsync(Expression<Func<Location, bool>> predicate);
        IEnumerable<Location> List(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
    }
}