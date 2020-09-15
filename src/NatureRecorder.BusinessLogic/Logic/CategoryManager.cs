using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.BusinessLogic.Extensions;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Data;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    internal class CategoryManager : ICategoryManager
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;
        private NatureRecorderFactory _factory;

        internal CategoryManager(NatureRecorderFactory factory)
        {
            _factory = factory;
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
            List<Category> categories = await _factory.Context.Categories
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
                results = _factory.Context.Categories;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _factory.Context.Categories
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
                results = _factory.Context.Categories;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Categories
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
                _factory.Context.Categories.Add(category);
                _factory.Context.SaveChanges();
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
                await _factory.Context.Categories.AddAsync(category);
                await _factory.Context.SaveChangesAsync();
            }

            return category;
        }

        /// <summary>
        /// Rename a category
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public Category Rename(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a category with the original name
            Category original = Get(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Category '{newName}' does not exist";
                throw new CategoryDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            Category newCategory = Get(s => s.Name == newName);
            if (newCategory != null)
            {
                string message = $"Category '{newName}' already exists";
                throw new CategoryAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            _factory.Context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Rename a category
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task<Category> RenameAsync(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a category with the original name
            Category original = await GetAsync(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Category '{newName}' does not exist";
                throw new CategoryDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            Category newCategory = await GetAsync(s => s.Name == newName);
            if (newCategory != null)
            {
                string message = $"Category '{newName}' already exists";
                throw new CategoryAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            await _factory.Context.SaveChangesAsync();

            return original;
        }

        /// <summary>
        /// Delete the category with the specified name
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            // Get the location and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Category category = Get(c => c.Name == name);

            if (category == null)
            {
                string message = $"Category '{name}' does not exist";
                throw new CategoryDoesNotExistException(message);
            }

            // Check the category isn't associated with any species
            Species species = _factory.Context
                                      .Species
                                      .FirstOrDefault(s => s.CategoryId == category.Id);

            if (species != null)
            {
                string message = $"Cannot delete category '{name}' while it contains species";
                throw new CategoryIsInUseException(message);
            }

            // Delete the category
            _factory.Context.Categories.Remove(category);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the category with the specified name
        /// </summary>
        /// <param name="name"></param>
        public async Task DeleteAsync(string name)
        {
            // Get the location and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Category category = await GetAsync(c => c.Name == name);

            if (category == null)
            {
                string message = $"Category '{name}' does not exist";
                throw new CategoryDoesNotExistException(message);
            }

            // Check the category isn't associated with any species
            Species species = await _factory.Context
                                            .Species
                                            .AsAsyncEnumerable()
                                            .FirstOrDefaultAsync(s => s.CategoryId == category.Id);

            if (species != null)
            {
                string message = $"Cannot delete category '{name}' while it contains species";
                throw new CategoryIsInUseException(message);
            }

            // Delete the category
            _factory.Context.Categories.Remove(category);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
