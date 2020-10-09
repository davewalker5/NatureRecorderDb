using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISpeciesStatusRatingManager
    {
        void ClearRating(string speciesName, string schemeName);
        Task ClearRatingAsync(string speciesName, string schemeName);
        SpeciesStatusRating Get(Expression<Func<SpeciesStatusRating, bool>> predicate);
        Task<SpeciesStatusRating> GetAsync(Expression<Func<SpeciesStatusRating, bool>> predicate);
        SpeciesStatusRating GetCurrent(string speciesName, string schemeName);
        Task<SpeciesStatusRating> GetCurrentAsync(string speciesName, string schemeName);
        IEnumerable<SpeciesStatusRating> List(Expression<Func<SpeciesStatusRating, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<SpeciesStatusRating> ListAsync(Expression<Func<SpeciesStatusRating, bool>> predicate, int pageNumber, int pageSize);
        SpeciesStatusRating SetRating(string speciesName, string ratingName, string schemeName);
        Task<SpeciesStatusRating> SetRatingAsync(string speciesName, string ratingName, string schemeName);
    }
}