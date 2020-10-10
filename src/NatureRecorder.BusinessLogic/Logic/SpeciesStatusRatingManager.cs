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
    internal class SpeciesStatusRatingManager : ISpeciesStatusRatingManager
    {
        private readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;
        private NatureRecorderFactory _factory;

        internal SpeciesStatusRatingManager(NatureRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the most recent rating for the specified species and scheme
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public SpeciesStatusRating GetCurrent(string speciesName, string schemeName)
        {
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());

            // Check the species and scheme exist
            Species species = _factory.Species.Get(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            StatusScheme scheme = _factory.StatusSchemes.Get(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException(message);
            }

            // Return the most recent entry for the specified species and scheme,
            // that should have an empty end date
            return Get(r => (r.Species.Name == speciesName) &&
                             (r.Rating.Scheme.Name == schemeName) &&
                             (r.End == null));
        }

        /// <summary>
        /// Return the most recent rating for the specified species and scheme
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public async Task<SpeciesStatusRating> GetCurrentAsync(string speciesName, string schemeName)
        {
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());

            // Check the species and scheme exist
            Species species = await _factory.Species.GetAsync(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            StatusScheme scheme = await _factory.StatusSchemes.GetAsync(s => s.Name == schemeName);
            if (scheme == null)
            {
                string message = $"Scheme '{schemeName}' does not exist";
                throw new StatusSchemeDoesNotExistException(message);
            }

            // Return the most recent entry for the specified species and scheme,
            // that should have an empty end date
            return await GetAsync(r => (r.Species.Name == speciesName) &&
                                       (r.Rating.Scheme.Name == schemeName) &&
                                       (r.End == null));
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SpeciesStatusRating Get(Expression<Func<SpeciesStatusRating, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<SpeciesStatusRating> GetAsync(Expression<Func<SpeciesStatusRating, bool>> predicate)
        {
            List<SpeciesStatusRating> ratings = await _factory.Context
                                                              .SpeciesStatusRatings
                                                              .Include(r => r.Species)
                                                              .ThenInclude(s => s.Category)
                                                              .Include(r => r.Rating)
                                                              .ThenInclude(r => r.Scheme)
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
        public IEnumerable<SpeciesStatusRating> List(Expression<Func<SpeciesStatusRating, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<SpeciesStatusRating> results;
            if (predicate == null)
            {
                results = _factory.Context.SpeciesStatusRatings
                                          .Include(r => r.Species)
                                          .ThenInclude(s => s.Category)
                                          .Include(r => r.Rating)
                                          .ThenInclude(r => r.Scheme)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.SpeciesStatusRatings
                                          .Include(r => r.Species)
                                          .ThenInclude(s => s.Category)
                                          .Include(r => r.Rating)
                                          .ThenInclude(r => r.Scheme)
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
        public IAsyncEnumerable<SpeciesStatusRating> ListAsync(Expression<Func<SpeciesStatusRating, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<SpeciesStatusRating> results;

            if (predicate == null)
            {
                results = _factory.Context.SpeciesStatusRatings
                                          .Include(r => r.Species)
                                          .ThenInclude(s => s.Category)
                                          .Include(r => r.Rating)
                                          .ThenInclude(r => r.Scheme)
                                          .AsAsyncEnumerable()
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize);
            }
            else
            {
                results = _factory.Context.SpeciesStatusRatings
                                          .Include(r => r.Species)
                                          .ThenInclude(s => s.Category)
                                          .Include(r => r.Rating)
                                          .ThenInclude(r => r.Scheme)
                                          .Where(predicate)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Set the conservation status rating for a species
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="ratingName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public SpeciesStatusRating SetRating(string speciesName, string ratingName, string schemeName)
        {
            // Tidy the parameters for direct comparison
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            ratingName = _textInfo.ToTitleCase(ratingName.CleanString());
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());

            // Check the species and rating exist
            Species species = _factory.Species.Get(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            StatusRating rating = _factory.StatusRatings.Get(s => (s.Name == ratingName) && (s.Scheme.Name == schemeName));
            if (rating == null)
            {
                string message = $"Status rating '{ratingName}' does not exist on scheme '{schemeName}'";
                throw new StatusRatingDoesNotExistException(message);
            }

            // See if there's already a rating for this species on the specified scheme
            SpeciesStatusRating speciesRating = _factory.Context
                                                        .SpeciesStatusRatings
                                                        .Include(r => r.Rating)
                                                        .FirstOrDefault(r => (r.SpeciesId == species.Id) &&
                                                                             (r.Rating.StatusSchemeId == rating.StatusSchemeId));
            if (speciesRating != null)
            {
                // There is an existing rating on this scheme, so set the end-date for
                // that rating to midnight yesterday
                speciesRating.End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-1);

                // This could leave a rating where the start date's greater than the end date. If that's the case,
                // make them the same
                if (speciesRating.Start > speciesRating.End)
                {
                    speciesRating.Start = speciesRating.End;
                }
            }

            // Create the new rating
            SpeciesStatusRating newRating = new SpeciesStatusRating
            {
                SpeciesId = species.Id,
                StatusRatingId = rating.Id,
                Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
            };
            _factory.Context.SpeciesStatusRatings.Add(newRating);
            _factory.Context.SaveChanges();

            // Load and return the new rating (to load related entities)
            return Get(r => r.Id == newRating.Id);
        }

        /// <summary>
        /// Set the conservation status rating for a species
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="ratingName"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public async Task<SpeciesStatusRating> SetRatingAsync(string speciesName, string ratingName, string schemeName)
        {
            // Tidy the parameters for direct comparison
            speciesName = _textInfo.ToTitleCase(speciesName.CleanString());
            ratingName = _textInfo.ToTitleCase(ratingName.CleanString());
            schemeName = _textInfo.ToTitleCase(schemeName.CleanString());

            // Check the species and rating
            Species species = await _factory.Species.GetAsync(s => s.Name == speciesName);
            if (species == null)
            {
                string message = $"Species '{speciesName}' does not exist";
                throw new SpeciesDoesNotExistException(message);
            }

            StatusRating rating = await _factory.StatusRatings.GetAsync(s => (s.Name == ratingName) && (s.Scheme.Name == schemeName));
            if (rating == null)
            {
                string message = $"Status rating '{ratingName}' does not exist on scheme '{schemeName}'";
                throw new StatusRatingDoesNotExistException(message);
            }

            // See if there's already a rating for this species on the specified scheme
            SpeciesStatusRating speciesRating = await _factory.Context
                                                              .SpeciesStatusRatings
                                                              .Include(r => r.Rating)
                                                              .FirstOrDefaultAsync(r => (r.SpeciesId == species.Id) &&
                                                                                        (r.Rating.StatusSchemeId == rating.StatusSchemeId));
            if (speciesRating != null)
            {
                // There is an existing rating on this scheme, so set the end-date for
                // that rating to midnight yesterday
                speciesRating.End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-1);

                // This could leave a rating where the start date's greater than the end date. If that's the case,
                // make them the same
                if (speciesRating.Start > speciesRating.End)
                {
                    speciesRating.Start = speciesRating.End;
                }
            }

            // Create the new rating
            SpeciesStatusRating newRating = new SpeciesStatusRating
            {
                SpeciesId = species.Id,
                StatusRatingId = rating.Id,
                Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
            };
            await _factory.Context.SpeciesStatusRatings.AddAsync(newRating);
            await _factory.Context.SaveChangesAsync();

            // Load and return the new rating (to load related entities)
            return await GetAsync(r => r.Id == newRating.Id);
        }

        /// <summary>
        /// Clear the rating for a species against a scheme
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="schemeName"></param>
        public void ClearRating(string speciesName, string schemeName)
        {
            SpeciesStatusRating rating = GetCurrent(speciesName, schemeName);
            if (rating != null)
            {
                rating.End = DateTime.Now;
                _factory.Context.SaveChanges();
            }
        }

        /// <summary>
        /// Clear the rating for a species against a scheme
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="schemeName"></param>
        public async Task ClearRatingAsync(string speciesName, string schemeName)
        {
            SpeciesStatusRating rating = await GetCurrentAsync(speciesName, schemeName);
            if (rating != null)
            {
                rating.End = DateTime.Now;
                await _factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Add a new conservation status rating based on the supplied template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public SpeciesStatusRating Add(SpeciesStatusRating template)
        {
            // Retrieve/create the category andspecies. The logic to create new
            // records or return existing ones is in these methods
            Species species = _factory.Species.Add(template.Species.Name, template.Species.Category.Name);

            // If the scheme doesn't exist, create it
            string schemeName = _textInfo.ToTitleCase(template.Rating.Scheme.Name.CleanString());
            StatusScheme scheme = _factory.StatusSchemes.Get(s => s.Name == schemeName);
            if (scheme == null)
            {
                scheme = _factory.StatusSchemes.Add(schemeName);
            }

            // If the rating doesn't exist, create it
            string ratingName = _textInfo.ToTitleCase(template.Rating.Name.CleanString());
            StatusRating rating = _factory.StatusRatings.Get(r => (r.Name == ratingName) && (r.StatusSchemeId == scheme.Id));
            if (rating == null)
            {
                rating = _factory.StatusRatings.Add(ratingName, schemeName);
            }

            // Add a new species status rating. Note that we DON'T use the SetRating
            // method as it manipulates prior records and we don't want to do that in
            // this context (import of rating data)
            SpeciesStatusRating newRating = new SpeciesStatusRating
            {
                SpeciesId = species.Id,
                StatusRatingId = rating.Id,
                Region = template.Region,
                Start = template.Start,
                End = template.End
            };
            _factory.Context.SpeciesStatusRatings.Add(newRating);
            _factory.Context.SaveChanges();

            // Load and return the new rating (to load related entities)
            return Get(r => r.Id == newRating.Id);
        }
    }
}
