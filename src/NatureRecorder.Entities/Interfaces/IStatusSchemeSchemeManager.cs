using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface IStatusSchemeSchemeManager
    {
        StatusScheme Add(string name);
        Task<StatusScheme> AddAsync(string name);
        void Delete(string name);
        Task DeleteAsync(string name);
        StatusScheme Get(Expression<Func<StatusScheme, bool>> predicate);
        Task<StatusScheme> GetAsync(Expression<Func<StatusScheme, bool>> predicate);
        IEnumerable<StatusScheme> List(Expression<Func<StatusScheme, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<StatusScheme> ListAsync(Expression<Func<StatusScheme, bool>> predicate, int pageNumber, int pageSize);
        StatusScheme Rename(string oldName, string newName);
        Task<StatusScheme> RenameAsync(string oldName, string newName);
    }
}