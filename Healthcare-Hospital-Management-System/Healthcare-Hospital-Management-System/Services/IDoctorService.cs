using System.Collections.Generic;
using HealthcareHospitalManagementSystem.Services;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Infrastructure;

namespace HealthcareHospitalManagementSystem.Services
{
    public interface IDoctorService
    {
        void AddDoctor(Doctor doctor, Logger Logger);
        List<Doctor> GetDoctors();
    }
}