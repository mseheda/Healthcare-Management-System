using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DoctorAppointmentControllerTests
    {
        private Mock<IDoctorService> _mockDoctorService;
        private DoctorAppointmentController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDoctorService = new Mock<IDoctorService>();
            _controller = new DoctorAppointmentController(_mockDoctorService.Object);
        }

        [TestMethod]
        public void AddTimeSlot_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.AddTimeSlot(null);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Invalid request.", badRequest.Value);
        }

        [TestMethod]
        public void AddTimeSlot_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new AddTimeSlotRequest
            {
                DoctorName = "Dr. Brown",
                Specialization = "Orthopedics",
                SlotDate = DateOnly.FromDateTime(new DateTime(2024, 12, 10)),
                SlotTime = new TimeOnly(10, 0),
                DurationInMinutes = 30
            };

            _mockDoctorService.Setup(s => s.AddTimeSlot("Dr. Brown", "Orthopedics",
                request.SlotDate.ToDateTime(request.SlotTime), TimeSpan.FromMinutes(30))).Returns(true);

            // Act
            var result = _controller.AddTimeSlot(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Time slot added successfully.", okResult.Value);
        }

        [TestMethod]
        public void AddTimeSlot_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var request = new AddTimeSlotRequest
            {
                DoctorName = "Dr. Brown",
                Specialization = "Orthopedics",
                SlotDate = DateOnly.FromDateTime(new DateTime(2024, 12, 10)),
                SlotTime = new TimeOnly(10, 0),
                DurationInMinutes = 30
            };

            _mockDoctorService.Setup(s => s.AddTimeSlot("Dr. Brown", "Orthopedics",
                request.SlotDate.ToDateTime(request.SlotTime), TimeSpan.FromMinutes(30))).Returns(false);

            // Act
            var result = _controller.AddTimeSlot(request);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Failed to add time slot. The doctor may not exist, or the time slot may already be added.", badRequest.Value);
        }

        [TestMethod]
        public void GetAvailableDoctors_ReturnsOk_WhenDoctorsAvailable()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Name = "Dr. Smith", Specialization = "Cardiology" }
            };

            _mockDoctorService.Setup(s => s.GetAvailableDoctors("Cardiology", It.IsAny<DateTime>(), null))
                              .Returns(doctors);

            // Act
            var result = _controller.GetAvailableDoctors("Cardiology", DateTime.Now, null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDoctors = okResult.Value as List<Doctor>;
            Assert.IsNotNull(returnedDoctors);
            Assert.AreEqual(1, returnedDoctors.Count);
        }

        [TestMethod]
        public void GetAvailableDoctors_ReturnsNotFound_WhenNoDoctors()
        {
            // Arrange
            _mockDoctorService.Setup(s => s.GetAvailableDoctors("Cardiology", It.IsAny<DateTime>(), null))
                              .Returns(new List<Doctor>());

            var date = new DateTime(2024, 12, 10);

            // Act
            var result = _controller.GetAvailableDoctors("Cardiology", date, null);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
            Assert.AreEqual($"No available doctors found for specialization Cardiology on {date:yyyy-MM-dd}", notFound.Value);
        }

        [TestMethod]
        public void ScheduleAppointment_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.ScheduleAppointment(null);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Invalid request.", badRequest.Value);
        }

        [TestMethod]
        public void ScheduleAppointment_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new ScheduleAppointmentRequest
            {
                DoctorName = "Dr. Jane",
                Specialization = "Neurology",
                AppointmentDate = DateOnly.FromDateTime(new DateTime(2024, 12, 10)),
                AppointmentTime = new TimeOnly(9, 0),
                DurationInMinutes = 30
            };

            var appointmentDateTime = request.AppointmentDate.ToDateTime(request.AppointmentTime);

            _mockDoctorService.Setup(s => s.ScheduleAppointment("Dr. Jane", "Neurology", appointmentDateTime, 30))
                              .Returns(true);

            // Act
            var result = _controller.ScheduleAppointment(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Appointment scheduled successfully.", okResult.Value);
        }

        [TestMethod]
        public void ScheduleAppointment_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var request = new ScheduleAppointmentRequest
            {
                DoctorName = "Dr. Jane",
                Specialization = "Neurology",
                AppointmentDate = DateOnly.FromDateTime(new DateTime(2024, 12, 10)),
                AppointmentTime = new TimeOnly(9, 0),
                DurationInMinutes = 30
            };

            var appointmentDateTime = request.AppointmentDate.ToDateTime(request.AppointmentTime);

            _mockDoctorService.Setup(s => s.ScheduleAppointment("Dr. Jane", "Neurology", appointmentDateTime, 30))
                              .Returns(false);

            // Act
            var result = _controller.ScheduleAppointment(request);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Failed to schedule appointment. The doctor may not be available at the specified date and time.", badRequest.Value);
        }
    }
}
