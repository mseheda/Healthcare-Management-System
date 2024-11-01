using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public class HealthcareDbContextFactory : IDesignTimeDbContextFactory<HealthcareDbContext>
    {
        public HealthcareDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HealthcareDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new HealthcareDbContext(optionsBuilder.Options);
        }
    }
}
