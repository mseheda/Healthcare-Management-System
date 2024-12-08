using Healthcare_Hospital_Management_System.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class ScheduleAppointmentRequestTests
    {
        [TestMethod]
        public void ScheduleAppointmentRequest_Properties_SetCorrectly()
        {
            // Arrange
            var request = new ScheduleAppointmentRequest
            {
                DoctorName = "Dr. Jane",
                Specialization = "Neurology",
                AppointmentDate = DateOnly.Parse("2024-12-10"),
                AppointmentTime = TimeOnly.Parse("15:30"),
                DurationInMinutes = 45
            };

            // Act & Assert
            Assert.AreEqual("Dr. Jane", request.DoctorName);
            Assert.AreEqual("Neurology", request.Specialization);
            Assert.AreEqual(DateOnly.Parse("2024-12-10"), request.AppointmentDate);
            Assert.AreEqual(TimeOnly.Parse("15:30"), request.AppointmentTime);
            Assert.AreEqual(45, request.DurationInMinutes);
        }
    }
}
