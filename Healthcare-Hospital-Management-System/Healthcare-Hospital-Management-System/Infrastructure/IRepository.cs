namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<T> GetByIdAsync(string reportSafetyId, CancellationToken cancellationToken);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);
        Task DeleteAsync(string reportSafetyIdd, CancellationToken cancellationToken);
    }
}