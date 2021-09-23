using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Research_Repository.Data;
using Research_Repository_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Repository
{
    public class Repository<T> : ControllerBase, IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        //Get single database instance of a generic type
        public T Find(int id)
        {
            return dbSet.Find(id);
        }

        //Get single database instance of a generic type
        public T FirstOrDefault(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            //Apply filter to query if the 'filter' parameter is not null
            if (filter != null)
            {
                query = query.Where(filter);
            }
            //Get navigational properties if the 'include' parameter is not null
            if (include != null)
            {
                    query = include(query);
            }
            //Set not tracking if 'isTracking' parameter is set to false
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            //Apply filter to query if the 'filter' parameter is not null
            if (filter != null)
            {
                query = query.Where(filter);
            }
            //Get navigational properties if the 'include' parameter is not null
            if (include != null)
            {
                query = include(query);
            }
            //Get ordered results if the 'orderBy' parameter is not null
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            //Set not tracking if 'isTracking' parameter is set to false
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
