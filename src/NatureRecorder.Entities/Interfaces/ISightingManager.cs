﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NatureRecorder.Entities.Db;
using NatureRecorder.Entities.Reporting;

namespace NatureRecorder.Entities.Interfaces
{
    public interface ISightingManager
    {
        Sighting Add(int number, Gender gender, bool withYoung, DateTime date, int locationId, int speciesId);
        Sighting Add(Sighting template);
        Task<Sighting> AddAsync(int number, Gender gender, bool withYoung, DateTime date, int locationId, int speciesId);
        void Delete(int id);
        Task DeleteAsync(int id);
        Sighting Get(Expression<Func<Sighting, bool>> predicate);
        Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate);
        IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByCategory(int categoryId, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListByCategoryAsync(int categoryId, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByDate(DateTime from, DateTime to, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListByDateAsync(DateTime from, DateTime to, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByLocation(int locationId, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListByLocationAsync(int locationId, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListBySpecies(int speciesId, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListBySpeciesAsync(int speciesId, int pageNumber, int pageSize);
        Summary Summarise(DateTime from, DateTime to, int? locationId, int? categoryId, int? speciesId);
        Task<Summary> SummariseAsync(DateTime from, DateTime to, int? locationId, int? categoryId, int? speciesId);
    }
}