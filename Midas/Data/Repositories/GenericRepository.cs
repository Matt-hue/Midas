using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Midas.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly ILogger _logger;
        private ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(
            ApplicationDbContext context,
            ILogger logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T?> GetMaxAsync2(Expression<Func<T, T>> selector)
        {
            T result = await _dbSet.MaxAsync(selector);
            return result;
        }

        //public virtual async Task<T?> GetMaxAsync(Func<T, DateTime> selector)
        //{
        //    var result = await _dbSet.MaxAsync(x => selector(x));
        //    return result;
        //}

        public virtual async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return true;
        }

        // public virtual Task<bool> Delete(Guid id)
        // {
        //    throw new NotImplementedException();
        // }
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<IEnumerable<T>> AllAsync()
        {
            return await _dbSet.ToListAsync<T>();
        }

        public async virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual IQueryable<T> QueryableFind(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async virtual Task ReloadEntityAsync(T entity)
        {
            await _dbSet.Entry(entity).ReloadAsync();
        }
    }
}
