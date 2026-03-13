using Application.Shared.Interfaces;
using Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Shared
{
    public class Repository : IRepository
    {
        private readonly BaseContext _context;

        public Repository(BaseContext context)
        {
            _context = context;
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(Guid id) where TEntity : class
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<bool> ExistsAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            return await Task.FromResult(_context.Set<TEntity>().Any(predicate));
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        async Task IRepository.UpdateAsync<TEntity>(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        async Task IRepository.DeleteAsync<TEntity>(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        IQueryable<T> IRepository.AsQueryable<T>(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        async Task<T> IRepository.SingleAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.SingleOrDefaultAsync();
        }
    }
}
