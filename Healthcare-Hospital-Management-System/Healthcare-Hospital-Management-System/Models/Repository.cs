using Healthcare_Hospital_Management_System.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Hospital_Management_System.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly HealthcareDbContext _context;

        public Repository(HealthcareDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }
        public async Task<T> GetByIdAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => EF.Property<string>(e, "SafetyReportId") == reportSafetyId, cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            var entity = await _context.Set<T>().FindAsync(new object[] { reportSafetyId }, cancellationToken);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
