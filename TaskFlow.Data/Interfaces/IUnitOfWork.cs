
using TaskFlow.Data.Interfaces;

namespace TaskFlow.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Get repository for any entity
        IGenericRepository<T> Repository<T>() where T : class;

        // Save all pending changes
        Task<int> SaveChangesAsync();
    }
}
