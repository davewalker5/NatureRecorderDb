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
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class SpeciesManager : ISpeciesManager
    {
        private readonly NatureRecorderFactory _factory;
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

        internal SpeciesManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first species matching the specified criteria along with the associated category
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Species Get(Expression<Func<Species, bool>> predicate)
            => _factory.Context.Species
                               .Include(m => m.Category)
                               .Where(predicate)
                               .FirstOrDefault();

        /// <summary>
        /// Get the first species matching the specified criteria along with the associated category
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Species> GetAsync(Expression<Func<Species, bool>> predicate)
        {
            List<Species> models = await _factory.Context.Species
                                                         .Where(predicate)
                                                         .ToListAsync();
            return models.FirstOrDefault();
        }

        /// <summary>
        /// Get the species matching the specified criteria along with the associated categories
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Species> List(Expression<Func<Species, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Species> species;

            if (predicate == null)
            {
                species = _factory.Context.Species
                                          .Include(m => m.Category)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                species = _factory.Context.Species
                                          .Include(m => m.Category)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .Where(predicate);
            }

            return species;
        }

        /// <summary>
        /// Get the species matching the specified criteria along with the associated categories
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Species> ListAsync(Expression<Func<Species, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Species> species;

            if (predicate == null)
            {
                species = _factory.Context.Species
                                          .Include(m => m.Category)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .AsAsyncEnumerable();
            }
            else
            {
                species = _factory.Context.Species
                                          .Include(m => m.Category)
                                          .Where(predicate)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .AsAsyncEnumerable();
            }

            return species;
        }

        /// <summary>
        /// Get the species for a named category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Species> ListByCategory(string categoryName, int pageNumber, int pageSize)
        {
            categoryName = _textInfo.ToTitleCase(categoryName.CleanString());
            IEnumerable<Species> models = _factory.Context.Species
                                                         .Include(m => m.Category)
                                                         .Where(m => m.Category.Name == categoryName)
                                                         .Skip((pageNumber - 1) * pageSize)
                                                         .Take(pageSize);
            return models;
        }

        /// <summary>
        /// Get the species for a named category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Species> ListByCategoryAsync(string categoryName, int pageNumber, int pageSize)
        {
            categoryName = _textInfo.ToTitleCase(categoryName.CleanString());
            IAsyncEnumerable<Species> models = _factory.Context.Species
                                                              .Include(m => m.Category)
                                                              .Where(m => m.Category.Name == categoryName)
                                                              .Skip((pageNumber - 1) * pageSize)
                                                              .Take(pageSize)
                                                              .AsAsyncEnumerable();
            return models;
        }

        /// <summary>
        /// Add a named species associated with the specified category, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Species Add(string name, string categoryName)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            Species species = Get(s => s.Name == name);

            if (species == null)
            {
                Category category = _factory.Categories.Add(categoryName);
                species = new Species
                {
                    Name = _textInfo.ToTitleCase(name.CleanString()),
                    CategoryId = category.Id
                };
                _factory.Context.Species.Add(species);
                _factory.Context.SaveChanges();
                _factory.Context.Entry(species).Reference(m => m.Category).Load();
            }

            return species;
        }

        /// <summary>
        /// Add a named species associated with the specified category, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<Species> AddAsync(string name, string categoryName)
        {
            categoryName = _textInfo.ToTitleCase(categoryName.CleanString());
            Species species = await GetAsync(a => a.Name == name);

            if (species == null)
            {
                Category category = await _factory.Categories.AddAsync(categoryName);
                species = new Species
                {
                    Name = _textInfo.ToTitleCase(name.CleanString()),
                    CategoryId = category.Id
                };
                await _factory.Context.Species.AddAsync(species);
                await _factory.Context.SaveChangesAsync();
                await _factory.Context.Entry(species).Reference(m => m.Category).LoadAsync();
            }

            return species;
        }
    }
}
