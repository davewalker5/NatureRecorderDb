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
    internal class StatusRatingManager : IStatusRatingManager
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;
        private NatureRecorderFactory _factory;

        internal StatusRatingManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StatusRating Get(Expression<Func<StatusRating, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<StatusRating> GetAsync(Expression<Func<StatusRating, bool>> predicate)
        {
            List<StatusRating> ratings = await _factory.Context.StatusRatings
                                                               .Include(r => r.Scheme)
                                                               .Where(predicate)
                                                               .ToListAsync();
            return ratings.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<StatusRating> List(Expression<Func<StatusRating, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<StatusRating> results;
            if (predicate == null)
            {
                results = _factory.Context.StatusRatings
                                          .Include(r => r.Scheme)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.StatusRatings
                                          .Include(r => r.Scheme)
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
        public IAsyncEnumerable<StatusRating> ListAsync(Expression<Func<StatusRating, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<StatusRating> results;

            if (predicate == null)
            {
                results = _factory.Context.StatusRatings
                                          .Include(r => r.Scheme)
                                          .AsAsyncEnumerable()
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.StatusRatings
                                          .Include(r => r.Scheme)
                                          .Where(predicate)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named conservation status rating, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public StatusRating Add(string name, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = _factory.StatusSchemes.Get(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // Clean up the rating name and see if it exists. If so, just return it.
            // If not, create a new entry and return that
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusRating rating = Get(r => (r.StatusSchemeId == scheme.Id) &&
                                           (r.Name == name));

            if (rating == null)
            {
                rating = new StatusRating { Name = name, StatusSchemeId = scheme.Id };
                _factory.Context.StatusRatings.Add(rating);
                _factory.Context.SaveChanges();
            }

            return rating;
        }

        /// <summary>
        /// Add a named conservation status rating, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public async Task<StatusRating> AddAsync(string name, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = await _factory.StatusSchemes.GetAsync(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // Clean up the rating name and see if it exists. If so, just return it.
            // If not, create a new entry and return that
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusRating rating = await GetAsync(r => (r.StatusSchemeId == scheme.Id) &&
                                                      (r.Name == name));

            if (rating == null)
            {
                rating = new StatusRating { Name = name, StatusSchemeId = scheme.Id };
                _factory.Context.StatusRatings.Add(rating);
                await _factory.Context.SaveChangesAsync();
            }

            return rating;
        }

        /// <summary>
        /// Rename a conservation status rating
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public StatusRating Rename(string oldName, string newName, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = _factory.StatusSchemes.Get(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a rating with the original name
            StatusRating original = Get(r => (r.StatusSchemeId == scheme.Id) &&
                                             (r.Name == oldName));
            if (original == null)
            {
                string message = $"Conservation status rating '{newName}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            StatusRating newRating = Get(r => (r.StatusSchemeId == scheme.Id) &&
                                              (r.Name == newName));
            if (newRating != null)
            {
                string message = $"Conservation status rating '{newName}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            _factory.Context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Rename a conservation status rating
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public async Task<StatusRating> RenameAsync(string oldName, string newName, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = await _factory.StatusSchemes.GetAsync(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            oldName = _textInfo.ToTitleCase(oldName.CleanString());
            newName = _textInfo.ToTitleCase(newName.CleanString());

            // There must be a rating with the original name
            StatusRating original = await GetAsync(r => (r.StatusSchemeId == scheme.Id) &&
                                                        (r.Name == oldName));
            if (original == null)
            {
                string message = $"Conservation status rating '{newName}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingDoesNotExistException();
            }

            // There can't be an existing category with the specified name
            StatusRating newRating = await GetAsync(r => (r.StatusSchemeId == scheme.Id) &&
                                                         (r.Name == newName));
            if (newRating != null)
            {
                string message = $"Conservation status rating '{newName}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingAlreadyExistsException();
            }

            // Update the name on the original
            original.Name = newName;
            await _factory.Context.SaveChangesAsync();

            return original;
        }

        /// <summary>
        /// Delete the conservation status rating with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schemeName"></param>
        public void Delete(string name, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = _factory.StatusSchemes.Get(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // Get the rating and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusRating rating = Get(r => r.Name == name);
            if (rating == null)
            {
                string message = $"Status rating '{name}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingDoesNotExistException();
            }

            // Check the rating isn't associated with any species ratings
            SpeciesStatusRating speciesRatings = _factory.Context
                                                         .SpeciesStatusRatings
                                                         .FirstOrDefault(sr => sr.StatusRatingId == rating.Id);

            if (speciesRatings != null)
            {
                string message = $"Cannot delete conservation status rating '{name}' on scheme '{scheme.Name}' while it is in use";
                throw new StatusRatingIsInUseException(message);
            }

            // Delete the rating
            _factory.Context.StatusRatings.Remove(rating);
            _factory.Context.SaveChanges();
        }

        /// <summary>
        /// Delete the conservation status rating with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schemeName"></param>
        public async Task DeleteAsync(string name, string schemeName)
        {
            // Get the scheme and make sure it exists
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());
            StatusScheme scheme = await _factory.StatusSchemes.GetAsync(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Status scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException();
            }

            // Get the rating and make sure it exists
            name = _textInfo.ToTitleCase(name.CleanString());
            StatusRating rating = await GetAsync(r => r.Name == name);
            if (rating == null)
            {
                string message = $"Status rating '{name}' does not exist on scheme '{scheme.Name}'";
                throw new StatusRatingDoesNotExistException();
            }

            // Check the rating isn't associated with any species ratings
            SpeciesStatusRating speciesRatings = await _factory.Context
                                                               .SpeciesStatusRatings
                                                               .AsAsyncEnumerable()
                                                               .FirstOrDefaultAsync(sr => sr.StatusRatingId == rating.Id);

            if (speciesRatings != null)
            {
                string message = $"Cannot delete conservation status rating '{name}' on scheme '{scheme.Name}' while it is in use";
                throw new StatusRatingIsInUseException(message);
            }

            // Delete the rating
            _factory.Context.StatusRatings.Remove(rating);
            await _factory.Context.SaveChangesAsync();
        }
    }
}
