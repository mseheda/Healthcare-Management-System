using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using HealthcareHospitalManagementSystem.Infrastructure;

public class DoctorService : IDoctorService
{
    private List<Doctor> _doctors;
    private readonly Logger _logger;
    private const int MaxDoctors = 50;
    public static int TotalDoctorsAdded { get; private set; } = 0;

    public DoctorService(Logger logger)
    {
        _doctors = new List<Doctor>();
        _logger = logger;
    }

    public void AddDoctor(Doctor doctor, Logger logger)
    {
        string message;
        if (_doctors.Count >= MaxDoctors)
        {
            message = $"Cannot add doctor {doctor.Name}. Maximum number of doctors reached.";
            logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        if (!ValidateDoctor(doctor))
        {
            message = $"Cannot add doctor {doctor.Name}. Invalid doctor details.";
            logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        message = $"Doctor {doctor.Name} added.";
        _doctors.Add(doctor);
        TotalDoctorsAdded++;
        logger.Log(message);
    }

    public List<Doctor> GetDoctors()
    {
        return _doctors;
    }

    public static bool ValidateDoctor(Doctor doctor)
    {
        return !string.IsNullOrWhiteSpace(doctor.Name) && !string.IsNullOrWhiteSpace(doctor.Specialization);
    }
}