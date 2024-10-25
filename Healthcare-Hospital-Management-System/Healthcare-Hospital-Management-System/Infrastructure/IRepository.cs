namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }
}
