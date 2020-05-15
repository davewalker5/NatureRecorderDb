using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISpeciesManager
    {
        Species Add(string name, string categoryName);
        Task<Species> AddAsync(string name, string categoryName);
        Species Get(Expression<Func<Species, bool>> predicate);
        Task<Species> GetAsync(Expression<Func<Species, bool>> predicate);
        IEnumerable<Species> List(Expression<Func<Species, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Species> ListAsync(Expression<Func<Species, bool>> predicate, int pageNumber, int pageSize);
        IEnumerable<Species> ListByCategory(string categoryName, int pageNumber, int pageSize);
        IAsyncEnumerable<Species> ListByCategoryAsync(string categoryName, int pageNumber, int pageSize);
    }
}