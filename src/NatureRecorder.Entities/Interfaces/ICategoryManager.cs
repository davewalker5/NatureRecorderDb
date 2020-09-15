using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ICategoryManager
    {
        Category Add(string name);
        Task<Category> AddAsync(string name);
        void Delete(string name);
        Task DeleteAsync(string name);
        Category Get(Expression<Func<Category, bool>> predicate);
        Task<Category> GetAsync(Expression<Func<Category, bool>> predicate);
        IEnumerable<Category> List(Expression<Func<Category, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Category> ListAsync(Expression<Func<Category, bool>> predicate, int pageNumber, int pageSize);
        Category Rename(string oldName, string newName);
        Task<Category> RenameAsync(string oldName, string newName);
    }
}