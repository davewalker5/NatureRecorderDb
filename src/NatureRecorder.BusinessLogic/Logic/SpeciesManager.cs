﻿using System;
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

        /// <summary>
        /// Rename a species
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public Species Rename(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a species with the original name
            Species original = Get(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Species '{newName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // There can't be an  existing species with the specified name
            Species newSpecies = Get(s => s.Name == newName);
            if (newSpecies != null)
            {
                string message = $"Species '{newName}' already exists";
                throw new SpeciesAlreadyExistsException(message);
            }

            // Update the name on the original
            original.Name = newName;
            _factory.Context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Rename a species
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task<Species> RenameAsync(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a species with the original name
            Species original = await GetAsync(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Species '{newName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // There can't be an  existing species with the specified name
            Species newSpecies = await GetAsync(s => s.Name == newName);
            if (newSpecies != null)
            {
                string message = $"Species '{newName}' already exists";
                throw new SpeciesAlreadyExistsException(message);
            }

            // Update the name on the original
            original.Name = newName;
            await _factory.Context.SaveChangesAsync();

            return original;
        }

        /// <summary>
        /// Move a species from one category to another
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Species Move(string speciesName, string categoryName)
        {
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            categoryName = _textInfo.ToTitleCase(categoryName.CleanString());

            // Find the species record
            Species species = Get(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // Find the  new category
            Category category = _factory.Categories.Get(s => s.Name == categoryName);
            if (category == null)
            {
                string message = $"Category '{categoryName}' does not exist";
                throw new CategoryDoesNotExistException(message);
            }

            // Check the species isn't already in that category
            if (species.CategoryId == category.Id)
            {
                string message = $"Species '{species.Name}' is already in category '{category.Name}'";
                throw new SpeciesIsAlreadyInCategoryException(message);
            }

            // Update the species record
            species.CategoryId = category.Id;
            _factory.Context.SaveChanges();

            // Re-get to ensure related entities are updated
            return Get(s => s.Id == species.Id);
        }

        /// <summary>
        /// Move a species from one category to another
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<Species> MoveAsync(string speciesName, string categoryName)
        {
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            categoryName = _textInfo.ToTitleCase(categoryName.CleanString());

            // Find the species record
            Species species = await GetAsync(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // Find the  new category
            Category category = await _factory.Categories.GetAsync(s => s.Name == categoryName);
            if (category == null)
            {
                string message = $"Category '{categoryName}' does not exist";
                throw new CategoryDoesNotExistException(message);
            }

            // Check the species isn't already in that category
            if (species.CategoryId == category.Id)
            {
                string message = $"Species '{species.Name}' is already in category '{category.Name}'";
                throw new SpeciesIsAlreadyInCategoryException(message);
            }

            // Update the species record
            species.CategoryId = category.Id;
            await _factory.Context.SaveChangesAsync();

            // Re-get to ensure related entities are updated
            return await GetAsync(s => s.Id == species.Id);
        }

        /// <summary>
        /// Delete the species with the specified name
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            // Get the species and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Species species = Get(l => l.Name == name);

            if (species == null)
            {
                string message = $"Species '{name}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // Check the species isn't used in any sightings
            Sighting sighting = _factory.Context
                                        .Sightings
                                        .FirstOrDefault(s => s.SpeciesId == species.Id);

            if (sighting != null)
            {
                string message = $"Cannot delete species '{name}' while it is referenced in sightings";
                throw new SpeciesIsInUseException(message);
            }

            // Delete the location
            _factory.Context.Species.Remove(species);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the species with the specified name
        /// </summary>
        /// <param name="name"></param>
        public async Task DeleteAsync(string name)
        {
            // Get the species and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            Species species = await GetAsync(l => l.Name == name);

            if (species == null)
            {
                string message = $"Species '{name}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            // Check the species isn't used in any sightings
            Sighting sighting = await _factory.Context
                                              .Sightings
                                              .AsAsyncEnumerable()
                                              .FirstOrDefaultAsync(s => s.SpeciesId == species.Id);

            if (sighting != null)
            {
                string message = $"Cannot delete speceis '{name}' while it is referenced in sightings";
                throw new SpeciesIsInUseException(message);
            }

            // Delete the species
            _factory.Context.Species.Remove(species);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
