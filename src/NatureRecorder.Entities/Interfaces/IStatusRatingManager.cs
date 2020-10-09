using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface IStatusRatingManager
    {
        StatusRating Add(string name, string schemeName);
        Task<StatusRating> AddAsync(string name, string schemeName);
        void Delete(string name, string schemeName);
        Task DeleteAsync(string name, string schemeName);
        StatusRating Get(Expression<Func<StatusRating, bool>> predicate);
        Task<StatusRating> GetAsync(Expression<Func<StatusRating, bool>> predicate);
        IEnumerable<StatusRating> List(Expression<Func<StatusRating, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<StatusRating> ListAsync(Expression<Func<StatusRating, bool>> predicate, int pageNumber, int pageSize);
        StatusRating Rename(string oldName, string newName, string schemeName);
        Task<StatusRating> RenameAsync(string oldName, string newName, string schemeName);
    }
}