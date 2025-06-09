﻿using DocumentFormat.OpenXml.InkML;
using Kpi.Domain.Commons;
using Kpi.Infrastructure.Contexts;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Kpi.Service.Service.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Auditable
    {
        protected readonly KpiDB dbContext;
        protected readonly DbSet<T> dbSet;
        public GenericRepository(KpiDB dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }
        public virtual async ValueTask<T> CreateAsync(T entity) => (await dbContext.AddAsync(entity)).Entity;

        public async ValueTask<bool> DeleteAsync(int id)
        {
            var entity = await GetAsync(e => e.Id == id);

            if (entity == null)
                return false;

            dbSet.Remove(entity);

            return true;
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null, string[] includes = null, bool isTracking = true)
        {
            var query = expression is null ? dbSet : dbSet.Where(expression);

            if (includes != null)
                foreach (var include in includes)
                    if (!string.IsNullOrEmpty(include))
                        query = query.Include(include);

            if (!isTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual async ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, bool isTracking = true, string[] includes = null) => await GetAll(expression, includes, isTracking).FirstOrDefaultAsync();

        public async ValueTask SaveChangesAsync() => await dbContext.SaveChangesAsync();

        public T UpdateAsync(T entity) => dbSet.Update(entity).Entity;

        public async ValueTask DeleteRangeAsync(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }

}
