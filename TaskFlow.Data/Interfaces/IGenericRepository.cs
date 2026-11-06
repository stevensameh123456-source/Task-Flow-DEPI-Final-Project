
using System.Linq.Expressions;

namespace TaskFlow.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Retrieve all records
        Task<List<T>> GetAllAsync();

        // Retrieve all records without tracking
        Task<List<T>> GetAllNoTrackingAsync();

        // Retrieve a record by its primary key
        Task<T?> GetByIdAsync(params object[] keyValues);

        // Add a new record
        Task AddAsync(T entity);

        // Update an existing record
        void Update(T entity);

        // Delete a record
        void Delete(T entity);

        // Check if a record exists
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
