using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Research_Repository_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Find(int id);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool isTracking = true
            );

        T FirstOrDefault(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool isTracking = true);

        void Add(T entity);

        void Remove(T entity);

        void Save();
    }
}
