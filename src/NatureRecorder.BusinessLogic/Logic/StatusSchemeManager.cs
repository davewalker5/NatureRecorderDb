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
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Entities.Interfaces;

namespace NatureRecorder.BusinessLogic.Logic
{
    public class StatusSchemeSchemeManager : IStatusSchemeSchemeManager
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;
        private NatureRecorderFactory _factory;

        internal StatusSchemeSchemeManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StatusScheme Get(Expression<Func<StatusScheme, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<StatusScheme> GetAsync(Expression<Func<StatusScheme, bool>> predicate)
        {
            List<StatusScheme> schemes = await _factory.Context.StatusSchemes
                                                               .Where(predicate)
                                                               .ToListAsync();
            return schemes.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<StatusScheme> List(Expression<Func<StatusScheme, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<StatusScheme> results;
            if (predicate == null)
            {
                results = _factory.Context.StatusSchemes
                                          .AsEnumerable()
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.StatusSchemes
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
        public IAsyncEnumerable<StatusScheme> ListAsync(Expression<Func<StatusScheme, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<StatusScheme> results;

            if (predicate == null)
            {
                results = _factory.Context.StatusSchemes
                                          .AsAsyncEnumerable()
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.StatusSchemes
                                          .Where(predicate)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named conservation status scheme, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StatusScheme Add(string name)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusScheme scheme = Get(a => a.Name == name);

            if (scheme == null)
            {
                scheme = new StatusScheme { Name = name };
                _factory.Context.StatusSchemes.Add(scheme);
                _factory.Context.SaveChanges();
            }

            return scheme;
        }

        /// <summary>
        /// Add a named conservation status scheme, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<StatusScheme> AddAsync(string name)
        {
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusScheme scheme = await GetAsync(a => a.Name == name);

            if (scheme == null)
            {
                scheme = new StatusScheme { Name = name };
                await _factory.Context.StatusSchemes.AddAsync(scheme);
                await _factory.Context.SaveChangesAsync();
            }

            return scheme;
        }

        /// <summary>
        /// Rename a conservation status scheme
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public StatusScheme Rename(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a category with the original name
            StatusScheme original = Get(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Conservation status scheme '{newName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            StatusScheme newScheme = Get(s => s.Name == newName);
            if (newScheme != null)
            {
                string message = $"Conservation status scheme '{newName}' already exists";
                throw new StatusSchemeAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            _factory.Context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Rename a conservation status scheme
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task<StatusScheme> RenameAsync(string oldName, string newName)
        {
            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a category with the original name
            StatusScheme original = await GetAsync(s => s.Name == oldName);
            if (original == null)
            {
                string message = $"Conservation status scheme '{newName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            StatusScheme newScheme = await GetAsync(s => s.Name == newName);
            if (newScheme != null)
            {
                string message = $"Conservation status scheme '{newName}' already exists";
                throw new StatusSchemeAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            await _factory.Context.SaveChangesAsync();

            return original;
        }

        /// <summary>
        /// Delete the conservation status scheme with the specified name
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            // Get the scheme and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusScheme scheme = Get(c => c.Name == name);

            if (scheme == null)
            {
                string message = $"Conservation status scheme '{name}' does not exist";
                throw new StatusSchemeDoesNotExistException(message);
            }

            // Check the scheme isn't associated with any species ratings
            SpeciesStatusRating rating = _factory.Context
                                                 .SpeciesStatusRatings
                                                 .Include(sr => sr.Rating)
                                                 .FirstOrDefault(sr => sr.Rating.StatusSchemeId == scheme.Id);

            if (rating != null)
            {
                string message = $"Cannot delete conservation status scheme '{name}' while it is in use";
                throw new StatusSchemeIsInUseException(message);
            }

            // Find all the ratings associated with this scheme and delete them
            IEnumerable<StatusRating> ratings = _factory.Context.StatusRatings;
            ratings = ratings.Where(r => r.StatusSchemeId == scheme.Id);
            _factory.Context.StatusRatings.RemoveRange(ratings);

            // Delete the scheme
            _factory.Context.StatusSchemes.Remove(scheme);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the conservation status scheme with the specified name
        /// </summary>
        /// <param name="name"></param>
        public async Task DeleteAsync(string name)
        {
            // Get the scheme and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusScheme scheme = await GetAsync(c => c.Name == name);

            if (scheme == null)
            {
                string message = $"Conservation status scheme '{name}' does not exist";
                throw new StatusSchemeDoesNotExistException(message);
            }

            // Check the scheme isn't associated with any species ratings
            SpeciesStatusRating rating = await _factory.Context
                                                       .SpeciesStatusRatings
                                                       .Include(sr => sr.Rating)
                                                       .AsAsyncEnumerable()
                                                       .FirstOrDefaultAsync(sr => sr.Rating.StatusSchemeId == scheme.Id);

            if (rating != null)
            {
                string message = $"Cannot delete conservation status scheme '{name}' while it is in use";
                throw new StatusSchemeIsInUseException(message);
            }

            // Find all the ratings associated with this scheme and delete them
            IEnumerable<StatusRating> ratings = _factory.Context.StatusRatings;
            ratings = ratings.Where(r => r.StatusSchemeId == scheme.Id);
            _factory.Context.StatusRatings.RemoveRange(ratings);

            // Delete the scheme
            _factory.Context.StatusSchemes.Remove(scheme);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
