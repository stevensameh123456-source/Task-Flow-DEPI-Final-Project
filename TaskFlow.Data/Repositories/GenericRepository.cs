
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskFlow.Data.Context;
using TaskFlow.Data.Interfaces;

namespace TaskFlow.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Retrieve all records with tracking
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Retrieve all records without tracking
        public async Task<List<T>> GetAllNoTrackingAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Retrieve a record by its primary key
        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        // Add a new record
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Update an existing record
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // Delete a record
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // Check if a record exists
        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
