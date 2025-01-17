using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
 public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected DbSet<T> dbSet;

        public Repository(ApplicationDbContext database)
        {
            _db = database;
            dbSet = _db.Set<T>();
        }

        public virtual void Add(T entity)
        {
            try
            {
                dbSet.Add(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add entity to database");
                throw new Exception("An error occurred while adding the entity.", ex);
            }
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false, Expression<Func<T, object>>? sortBy = null)
        {
            try
            {
                IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(property);
                    }
                }

                if (sortBy != null)
                {
                    query = query.OrderBy(sortBy);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve entities from the database");
                throw new Exception("An error occurred while retrieving the entities.", ex);
            }
        }

        public async virtual Task<T?>  GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            try
            {
                IQueryable<T> query;

                if (tracked)
                {
                    query = dbSet.Where(filter);
                }
                else
                {
                    query = dbSet.AsNoTracking().Where(filter);
                }

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(property);
                    }
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get entity from database");
                throw new Exception("An error occurred while retrieving the entity.", ex);
            }
        }

        public void Remove(T entity)
        {
            try
            {
                dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove entity from database");
                throw new Exception("An error occurred while removing the entity.", ex);
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update entity in database");
                throw new Exception("An error occurred while updating the entity.", ex);
            }
        }

        public virtual void UpdateRange(IEnumerable<T> entities) 
        {
            try
            {
                dbSet.UpdateRange(entities);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update entities in database");
                throw new Exception("An error occurred while updating the enties.", ex);
            }
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove range of entities from database");
                throw new Exception("An error occurred while removing the entities.", ex);
            }
        }
    }
}
