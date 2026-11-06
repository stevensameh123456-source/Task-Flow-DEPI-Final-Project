
using TaskFlow.Data.Context;
using TaskFlow.Data.Interfaces;
using TaskFlow.Data.Repositories;

namespace TaskFlow.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Get or Create Repository
        public IGenericRepository<T> Repository<T>()
            where T : class
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<T>(_context);
                _repositories[type] = repository;
            }

            return (IGenericRepository<T>)repository;
        }

        // Save All Pending Changes
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // DI Container manages DbContext lifetime.
        public void Dispose()
        {
            _repositories.Clear();
        }
    }
}
