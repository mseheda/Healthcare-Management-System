using HealthcareHospitalManagementSystem.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DoctorTests
    {
        [TestMethod]
        public void Doctor_DisplayInfo_ReturnsCorrectInfo()
        {
            // Arrange
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Age = 45,
                Specialization = "Cardiology",
                Department = "Internal Medicine"
            };

            // Act
            var result = doctor.DisplayInfo();

            // Assert
            Assert.AreEqual("Doctor Dr. Smith, Age: 45, Specialization: Cardiology, Department: Internal Medicine", result);
        }

        [TestMethod]
        public void Doctor_Equals_ReturnsTrue_ForSameDoctors()
        {
            // Arrange
            var doctor1 = new Doctor
            {
                Name = "Dr. Smith",
                Age = 45,
                Specialization = "Cardiology",
                Department = "Internal Medicine"
            };

            var doctor2 = new Doctor
            {
                Name = "Dr. Smith",
                Age = 45,
                Specialization = "Cardiology",
                Department = "Internal Medicine"
            };

            // Act
            var isEqual = doctor1.Equals(doctor2);

            // Assert
            Assert.IsTrue(isEqual);
        }
    }
}
