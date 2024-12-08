using HealthcareHospitalManagementSystem.Models;

namespace HealthcareHospitalManagementSystem.Services
{
    public interface IDoctorService
    {
        void AddDoctor(Doctor doctor, INotificationService notificationService);
        List<Doctor> GetDoctors();
        List<Doctor> GetAvailableDoctors(string specialization, DateTime date, TimeSpan? time = null);
        bool ScheduleAppointment(string doctorName, string specialization, DateTime date, int durationInMinutes);
        public bool AddTimeSlot(string doctorName, string specialization, DateTime date, TimeSpan time);
    }
}