using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;

public class DoctorService : IDoctorService
{
    private static List<Doctor> _doctors;
    private const int MaxDoctors = 50;
    public static int TotalDoctorsAdded { get; private set; } = 0;
    public HealthcareLogger Logger { get; set; }

    static DoctorService()
    {
        _doctors = new List<Doctor>();
    }

    public void AddDoctor(Doctor doctor, NotificationService? notificationService)
    {
        string message;

        if (_doctors.Count >= MaxDoctors)
        {
            message = $"Cannot add doctor {doctor.Name}. Maximum number of doctors reached.";
            Logger?.LogError(message);
            throw new InvalidOperationException(message);
        }

        if (!ValidateDoctor(doctor))
        {
            message = $"Cannot add doctor {doctor.Name}. Invalid doctor details.";
            Logger?.LogError(message);
            throw new InvalidOperationException(message);
        }

        _doctors.Add(doctor);
        TotalDoctorsAdded++;
        message = $"Doctor {doctor.Name} has been successfully added.";
        Logger?.Log(message);

        notificationService?.SendNotificationDoctorAsync(message, CancellationToken.None);
    }

    public List<Doctor> GetDoctors()
    {
        return _doctors;
    }

    public static bool ValidateDoctor(Doctor doctor)
    {
        return !string.IsNullOrWhiteSpace(doctor.Name) && !string.IsNullOrWhiteSpace(doctor.Specialization);
    }

    public List<Doctor> GetAvailableDoctors(string specialization, DateTime date, TimeSpan? time = null)
    {
        var dateTimeFilter = time.HasValue ? date.Date + time.Value : (DateTime?)null;

        return _doctors.Where(d =>
            d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase) &&
            d.AvailableDates.Any(slot =>
                slot.Date == date.Date && (!dateTimeFilter.HasValue || slot == dateTimeFilter)))
        .ToList();
    }

    public bool ScheduleAppointment(string doctorName, string specialization, DateTime date, TimeSpan time)
    {
        var doctor = _doctors.FirstOrDefault(d =>
            d.Name.Equals(doctorName, StringComparison.OrdinalIgnoreCase) &&
            d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase));

        if (doctor == null)
        {
            return false;
        }

        var dateTimeSlot = date.Date + time;

        if (!doctor.AvailableDates.Contains(dateTimeSlot))
        {
            return false;
        }

        doctor.AvailableDates.Remove(dateTimeSlot);
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

        var newDateTime = date.Date + time;

        if (doctor.AvailableDates.Contains(newDateTime))
        {
            return false;
        }

        doctor.AvailableDates.Add(newDateTime);
        return true;
    }
}
