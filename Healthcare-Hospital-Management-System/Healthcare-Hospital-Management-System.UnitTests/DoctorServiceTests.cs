using HealthcareHospitalManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;

namespace HealthcareHospitalManagementSystem.Tests.Services
{
    [TestClass]
    public class DoctorServiceTests
    {
        private DoctorService _doctorService;
        private Mock<IHealthcareLogger> _mockLogger;
        private Mock<INotificationService> _mockNotificationService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Reset the static fields before each test using reflection or re-instantiate the service
            // Assuming we can clear the static list via reflection:
            var doctorsField = typeof(DoctorService).GetField("_doctors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            doctorsField.SetValue(null, new List<Doctor>());

            // Reset TotalDoctorsAdded
            var totalDoctorsField = typeof(DoctorService).GetProperty("TotalDoctorsAdded", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            totalDoctorsField.SetValue(null, 0);

            _doctorService = new DoctorService();
            _mockLogger = new Mock<IHealthcareLogger>();
            _mockNotificationService = new Mock<INotificationService>();
        }

        [TestMethod]
        public void AddDoctor_ShouldAddDoctor_WhenValid()
        {
            var doctor = new Doctor { Name = "Dr. Smith", Specialization = "Cardiology" };

            _doctorService.Logger = _mockLogger.Object;
            _doctorService.AddDoctor(doctor, _mockNotificationService.Object);

            var doctors = _doctorService.GetDoctors();
            Assert.AreEqual(1, doctors.Count, "One doctor should have been added.");
            Assert.AreEqual(doctor, doctors.First(), "The added doctor should be the same instance.");

            _mockLogger.Verify(l => l.Log("Doctor Dr. Smith has been successfully added."), Times.Once);
            _mockNotificationService.Verify(n => n.SendNotificationDoctorAsync("Doctor Dr. Smith has been successfully added.", It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDoctor_ShouldThrow_WhenDoctorIsNull()
        {
            _doctorService.AddDoctor(null, _mockNotificationService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddDoctor_ShouldThrow_WhenDoctorAlreadyExists()
        {
            var doctor = new Doctor { Name = "Dr. Smith", Specialization = "Cardiology" };
            _doctorService.AddDoctor(doctor, null);

            // Adding the same doctor again should throw
            _doctorService.AddDoctor(new Doctor { Name = "Dr. Smith", Specialization = "Cardiology" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddDoctor_ShouldThrow_WhenMaxDoctorsReached()
        {
            // Add 50 doctors
            for (int i = 0; i < 50; i++)
            {
                _doctorService.AddDoctor(new Doctor { Name = $"Dr.{i}", Specialization = "General" }, null);
            }

            // Try adding one more
            _doctorService.AddDoctor(new Doctor { Name = "Extra Doctor", Specialization = "General" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddDoctor_ShouldThrow_WhenInvalidDetails()
        {
            // Invalid because specialization is empty
            var invalidDoctor = new Doctor { Name = "Dr. NoSpec", Specialization = "" };
            _doctorService.AddDoctor(invalidDoctor, null);
        }

        [TestMethod]
        public void ValidateDoctor_ShouldReturnTrue_WhenValidDoctor()
        {
            var validDoctor = new Doctor { Name = "Dr. Jones", Specialization = "Dermatology" };
            Assert.IsTrue(DoctorService.ValidateDoctor(validDoctor), "Should return true for a valid doctor.");
        }

        [TestMethod]
        public void ValidateDoctor_ShouldReturnFalse_WhenInvalidDoctor()
        {
            var invalidDoctor = new Doctor { Name = "", Specialization = "Neuro" };
            Assert.IsFalse(DoctorService.ValidateDoctor(invalidDoctor), "Should return false for a doctor with empty name.");
        }

        [TestMethod]
        public void GetAvailableDoctors_ShouldReturnOnlyDoctorsWithMatchingSlots()
        {
            var doctor1 = new Doctor
            {
                Name = "Dr. Brown",
                Specialization = "Orthopedics",
                AvailableTimeSlots = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Date = DateOnly.FromDateTime(new DateTime(2024,12,06)),
                        StartTime = TimeOnly.FromTimeSpan(new TimeSpan(10,0,0)),
                        DurationInMinutes = 30
                    }
                }
            };

            var doctor2 = new Doctor
            {
                Name = "Dr. Green",
                Specialization = "Orthopedics",
                AvailableTimeSlots = new List<TimeSlot>()
            };

            _doctorService.AddDoctor(doctor1, null);
            _doctorService.AddDoctor(doctor2, null);

            var results = _doctorService.GetAvailableDoctors("Orthopedics", new DateTime(2024, 12, 06), new TimeSpan(10, 0, 0));
            Assert.AreEqual(1, results.Count, "Only one doctor should match the given date/time.");
            Assert.AreEqual(doctor1, results.First(), "Dr. Brown should be the only available doctor.");
        }

        [TestMethod]
        public void ScheduleAppointment_ShouldReturnTrue_WhenSlotMatches()
        {
            var doctor = new Doctor
            {
                Name = "Dr. Patel",
                Specialization = "ENT",
                AvailableTimeSlots = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Date = DateOnly.FromDateTime(new DateTime(2024,12,06)),
                        StartTime = TimeOnly.FromTimeSpan(new TimeSpan(9,0,0)),
                        DurationInMinutes = 30
                    }
                }
            };

            _doctorService.AddDoctor(doctor, null);

            bool result = _doctorService.ScheduleAppointment("Dr. Patel", "ENT", new DateTime(2024, 12, 06, 9, 0, 0), 30);
            Assert.IsTrue(result, "Should return true if the timeslot matches exactly.");

            // Slot should now be removed
            Assert.AreEqual(0, doctor.AvailableTimeSlots.Count, "The timeslot should have been removed after scheduling.");
        }

        [TestMethod]
        public void ScheduleAppointment_ShouldReturnFalse_WhenNoMatchingDoctor()
        {
            bool result = _doctorService.ScheduleAppointment("NonExistent", "ENT", DateTime.Now, 30);
            Assert.IsFalse(result, "Should return false if no matching doctor is found.");
        }

        [TestMethod]
        public void ScheduleAppointment_ShouldReturnFalse_WhenNoMatchingSlot()
        {
            var doctor = new Doctor
            {
                Name = "Dr. White",
                Specialization = "ENT",
                AvailableTimeSlots = new List<TimeSlot>()
            };

            _doctorService.AddDoctor(doctor, null);

            // No slots available
            bool result = _doctorService.ScheduleAppointment("Dr. White", "ENT", DateTime.Now, 30);
            Assert.IsFalse(result, "Should return false if no matching slot is found.");
        }

        [TestMethod]
        public void AddTimeSlot_ShouldReturnTrue_WhenSlotIsAddedSuccessfully()
        {
            var doctor = new Doctor
            {
                Name = "Dr. Clark",
                Specialization = "Surgery"
            };

            _doctorService.AddDoctor(doctor, null);

            bool result = _doctorService.AddTimeSlot("Dr. Clark", "Surgery", new DateTime(2024, 12, 06, 11, 0, 0), new TimeSpan(0, 30, 0));
            Assert.IsTrue(result, "Should return true if a new time slot is successfully added.");

            Assert.AreEqual(1, doctor.AvailableTimeSlots.Count);
            Assert.AreEqual(11, doctor.AvailableTimeSlots.First().StartTime.Hour);
            Assert.AreEqual(30, doctor.AvailableTimeSlots.First().DurationInMinutes);
        }

        [TestMethod]
        public void AddTimeSlot_ShouldReturnFalse_WhenDoctorNotFound()
        {
            bool result = _doctorService.AddTimeSlot("NonExistent", "Surgery", DateTime.Now, new TimeSpan(0, 30, 0));
            Assert.IsFalse(result, "Should return false if no doctor matches.");
        }

        [TestMethod]
        public void AddTimeSlot_ShouldReturnFalse_WhenSlotAlreadyExists()
        {
            var doctor = new Doctor
            {
                Name = "Dr. Clark",
                Specialization = "Surgery",
                AvailableTimeSlots = new List<TimeSlot>
                {
                    new TimeSlot
                    {
                        Date = DateOnly.FromDateTime(new DateTime(2024,12,06)),
                        StartTime = TimeOnly.FromTimeSpan(new TimeSpan(11,0,0)),
                        DurationInMinutes = 30
                    }
                }
            };

            _doctorService.AddDoctor(doctor, null);

            // Attempt to add the same slot again
            bool result = _doctorService.AddTimeSlot("Dr. Clark", "Surgery", new DateTime(2024, 12, 06, 11, 0, 0), new TimeSpan(0, 30, 0));
            Assert.IsFalse(result, "Should return false if the slot already exists.");
        }

        [TestMethod]
        public void TotalDoctorsAdded_ShouldIncrementOnAddDoctor()
        {
            Assert.AreEqual(0, DoctorService.TotalDoctorsAdded, "Initially should be 0.");
            _doctorService.AddDoctor(new Doctor { Name = "Dr. Test", Specialization = "General" }, null);
            Assert.AreEqual(1, DoctorService.TotalDoctorsAdded, "Should increment after adding a doctor.");
        }
    }
}
