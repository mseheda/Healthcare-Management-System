using Healthcare_Hospital_Management_System.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class AddTimeSlotRequestTests
    {
        [TestMethod]
        public void AddTimeSlotRequest_Properties_SetCorrectly()
        {
            // Arrange
            var request = new AddTimeSlotRequest
            {
                DoctorName = "Dr. Smith",
                Specialization = "Cardiology",
                SlotDate = DateOnly.Parse("2024-12-01"),
                SlotTime = TimeOnly.Parse("14:00"),
                DurationInMinutes = 30
            };

            // Act & Assert
            Assert.AreEqual("Dr. Smith", request.DoctorName);
            Assert.AreEqual("Cardiology", request.Specialization);
            Assert.AreEqual(DateOnly.Parse("2024-12-01"), request.SlotDate);
            Assert.AreEqual(TimeOnly.Parse("14:00"), request.SlotTime);
            Assert.AreEqual(30, request.DurationInMinutes);
        }
    }
}
