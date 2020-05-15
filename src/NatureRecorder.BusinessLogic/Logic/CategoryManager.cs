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
    internal class CategoryManager : ICategoryManager
    {
        private readonly NatureRecorderDbContext _context;
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        internal CategoryManager(NatureRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Category Get(Expression<Func<Category, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Category> GetAsync(Expression<Func<Category, bool>> predicate)
        {
            List<Category> categories = await _context.Categories
                                                     .Where(predicate)
                                                     .ToListAsync();
            return categories.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<Category> List(Expression<Func<Category, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Category> results;
            if (predicate == null)
            {
                results = _context.Categories;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _context.Categories
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
        public virtual IAsyncEnumerable<Category> ListAsync(Expression<Func<Category, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Category> results;
            if (predicate == null)
            {
                results = _context.Categories;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Categories
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named category, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Category Add(string name)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            Category category = Get(a => a.Name == name);

            if (category == null)
            {
                category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();
            }

            return category;
        }

        /// <summary>
        /// Add a named category, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Category> AddAsync(string name)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            Category category = await GetAsync(a => a.Name == name);

            if (category == null)
            {
                category = new Category { Name = name };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }

            return category;
        }
    }
}
