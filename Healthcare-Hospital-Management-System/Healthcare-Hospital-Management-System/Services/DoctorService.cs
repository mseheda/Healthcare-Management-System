using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DoctorService : IDoctorService
{
    private static readonly List<Doctor> _doctors = new();
    private const int MaxDoctors = 50;

    public static int TotalDoctorsAdded { get; private set; } = 0;
    public IHealthcareLogger? Logger { get; set; }
    private readonly IDataProtectService _dataProtectService;

    public DoctorService(IDataProtectService dataProtectService)
    {
        _dataProtectService = dataProtectService ?? throw new ArgumentNullException(nameof(dataProtectService));
    }

    public void AddDoctor(Doctor doctor, INotificationService? notificationService = null)
    {
        if (doctor == null)
        {
            throw new ArgumentNullException(nameof(doctor));
        }

        if (_doctors.Any(d => d.Name.Equals(doctor.Name, StringComparison.OrdinalIgnoreCase) &&
                              d.Specialization.Equals(doctor.Specialization, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Doctor {doctor.Name} with specialization {doctor.Specialization} already exists.");
        }

        if (_doctors.Count >= MaxDoctors)
        {
            throw new InvalidOperationException("Maximum number of doctors reached.");
        }

        if (!ValidateDoctor(doctor))
        {
            throw new InvalidOperationException("Invalid doctor details.");
        }

        _doctors.Add(doctor);
        TotalDoctorsAdded++;

        var message = $"Doctor {doctor.Name} has been successfully added.";
        Logger?.Log(message);

        notificationService?.SendNotificationDoctorAsync(message, CancellationToken.None);
    }

    public List<Doctor> GetDoctors()
    {
        try
        {
            string encryptedFilePath = "transactions.log";
            string plainTextFilePath = "listofdoctors.log";

            if (File.Exists(encryptedFilePath))
            {
                byte[] encryptedData = File.ReadAllBytes(encryptedFilePath);
                string decryptedContent = _dataProtectService.DecryptAsync(encryptedData, CancellationToken.None).Result;

                File.WriteAllText(plainTextFilePath, decryptedContent);
            }
        }
        catch (Exception ex)
        {
            Logger?.Log($"Error decrypting transactions log: {ex.Message}");
        }

        return _doctors;
    }

    public static bool ValidateDoctor(Doctor doctor)
    {
        return !string.IsNullOrWhiteSpace(doctor.Name) && !string.IsNullOrWhiteSpace(doctor.Specialization);
    }

    public List<Doctor> GetAvailableDoctors(string specialization, DateTime date, TimeSpan? time = null)
    {
        if (string.IsNullOrWhiteSpace(specialization))
        {
            throw new ArgumentException("Specialization cannot be null or empty.", nameof(specialization));
        }

        var searchCriteria = (specialization, DateOnly.FromDateTime(date), time.HasValue ? TimeOnly.FromTimeSpan(time.Value) : (TimeOnly?)null);

        return _doctors
            .Where(d =>
                d.Specialization.Equals(searchCriteria.specialization, StringComparison.OrdinalIgnoreCase) &&
                d.AvailableTimeSlots.Any(slot =>
                    slot.Date == searchCriteria.Item2 &&
                    (!searchCriteria.Item3.HasValue || slot.StartTime == searchCriteria.Item3)))
            .ToList();
    }

    public bool ScheduleAppointment(string doctorName, string specialization, DateTime date, int durationInMinutes)
    {
        var doctor = _doctors.FirstOrDefault(d =>
            d.Name.Equals(doctorName, StringComparison.OrdinalIgnoreCase) &&
            d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase));

        if (doctor == null)
        {
            return false;
        }

        var appointmentSlot = new TimeSlot
        {
            Date = DateOnly.FromDateTime(date),
            StartTime = TimeOnly.FromDateTime(date),
            DurationInMinutes = durationInMinutes
        };

        var matchingSlot = doctor.AvailableTimeSlots.FirstOrDefault(slot =>
            slot.Date == appointmentSlot.Date &&
            slot.StartTime == appointmentSlot.StartTime &&
            slot.DurationInMinutes == appointmentSlot.DurationInMinutes);

        if (matchingSlot == null)
        {
            return false;
        }

        doctor.AvailableTimeSlots.Remove(matchingSlot);
        return true;
    }

    public bool AddTimeSlot(string doctorName, string specialization, DateTime date, TimeSpan time)
    {
        var doctor = _doctors.FirstOrDefault(d =>
            d.Name.Equals(doctorName, StringComparison.OrdinalIgnoreCase) &&
            d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase));

        if (doctor == null)
        {
            return false;
        }

        var newSlot = new TimeSlot
        {
            Date = DateOnly.FromDateTime(date),
            StartTime = TimeOnly.FromDateTime(date),
            DurationInMinutes = (int)time.TotalMinutes
        };

        if (doctor.AvailableTimeSlots.Any(slot =>
            slot.Date == newSlot.Date &&
            slot.StartTime == newSlot.StartTime))
        {
            return false;
        }

        doctor.AvailableTimeSlots.Add(newSlot);
        return true;
    }
}
