using HealthcareHospitalManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public class HealthcareDbContext : DbContext
    {
        public HealthcareDbContext(DbContextOptions<HealthcareDbContext> options)
            : base(options)
        {
        }

        public DbSet<DrugReportClass> DrugReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DrugReportClass>(entity =>
            {
                entity.HasKey(e => e.SafetyReportId);
                entity.Property(e => e.SafetyReportId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ReportDate).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PrimarySourceCountry).HasMaxLength(100);
                entity.Property(e => e.ReportType).HasMaxLength(50);
                entity.Property(e => e.Serious).HasMaxLength(20);
                entity.Property(e => e.ReporterQualification).HasMaxLength(100);
                entity.Property(e => e.PatientGender).HasMaxLength(10);
                entity.Property(e => e.SenderOrganization).HasMaxLength(200);
                entity.Property(e => e.ReceiverOrganization).HasMaxLength(200);
            });
        }
    }
}
