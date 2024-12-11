using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DoctorsControllerTests
    {
        private Mock<IDoctorService> _mockDoctorService;
        private Mock<IHealthcareLogger> _mockLogger;
        private Mock<INotificationService> _mockNotificationService;
        private DoctorsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDoctorService = new Mock<IDoctorService>();
            _mockLogger = new Mock<IHealthcareLogger>();
            _mockNotificationService = new Mock<INotificationService>();
            _controller = new DoctorsController(_mockDoctorService.Object, _mockLogger.Object, _mockNotificationService.Object);
        }

        [TestMethod]
        public void AddDoctor_ReturnsOk_WhenDoctorIsAddedSuccessfully()
        {
            // Arrange
            _mockDoctorService.Setup(s => s.AddDoctor(It.IsAny<Doctor>(), It.IsAny<INotificationService>()));

            // Act
            var result = _controller.AddDoctor();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            _mockDoctorService.Verify(s => s.AddDoctor(It.IsAny<Doctor>(), It.IsAny<INotificationService>()), Times.Once);
        }

        [TestMethod]
        public void AddDoctor_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockDoctorService
                .Setup(s => s.AddDoctor(It.IsAny<Doctor>(), It.IsAny<INotificationService>()))
                .Throws(new Exception("An error occurred"));

            // Act
            var result = _controller.AddDoctor();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred", objectResult.Value);
        }

        [TestMethod]
        public void GetDoctors_ReturnsOkWithDoctors_WhenDoctorsExist()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Name = "Dr. Smith", Specialization = "Cardiology" },
                new Doctor { Name = "Dr. Jane", Specialization = "Neurology" }
            };
            _mockDoctorService.Setup(s => s.GetDoctors()).Returns(doctors);

            // Act
            var result = _controller.GetDoctors();

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult, "Expected ObjectResult when doctors exist");
            Assert.AreEqual(200, objectResult.StatusCode, "Expected 200 OK status code");
            var returnedDoctors = objectResult.Value as List<Doctor>;
            Assert.IsNotNull(returnedDoctors, "Expected list of doctors");
            Assert.AreEqual(2, returnedDoctors.Count, "Should return the correct number of doctors");
        }

        [TestMethod]
        public void GetDoctors_ReturnsNotFound_WhenNoDoctorsExist()
        {
            // Arrange
            _mockDoctorService.Setup(s => s.GetDoctors()).Returns(new List<Doctor>());

            // Act
            var result = _controller.GetDoctors();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult when no doctors");
            Assert.AreEqual("No doctors found", notFoundResult.Value);
        }

        [TestMethod]
        public void GetDoctors_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockDoctorService.Setup(s => s.GetDoctors()).Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetDoctors();

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Database error", objectResult.Value);
        }
    }
}
