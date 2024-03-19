using System.Linq.Expressions;

namespace Midas.Data.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> AllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<bool> AddAsync(T entity);

        Task<bool> AddRangeAsync(IEnumerable<T> entities);

        void Delete(T entity);

        void Update(T entity);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task ReloadEntityAsync(T entity);

        IQueryable<T> QueryableFind(Expression<Func<T, bool>> predicate);
        //Task<T?> GetMaxAsync(Func<T, DateTime> selector);
    }
}
