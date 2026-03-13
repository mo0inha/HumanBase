using System.Linq.Expressions;

namespace Application.Shared.Interfaces
{
    public interface IRepository
    {
        Task<TEntity> GetByIdAsync<TEntity>(Guid id) where TEntity : class;
        Task<bool> ExistsAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : class;
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task SaveChangesAsync();

        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

        IQueryable<T> AsQueryable<T>(params Expression<Func<T, object>>[] includes) where T : class;
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class;
    }
}
