using Healthcare_Hospital_Management_System.Controllers;
using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IJwtService> _mockJwtService;
        private Mock<IUserService> _mockUserService;
        private AuthController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockJwtService = new Mock<IJwtService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new AuthController(_mockJwtService.Object, _mockUserService.Object);
        }

        [TestMethod]
        public void Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByEmail(It.IsAny<string>())).Returns((User?)null);

            // Act
            var result = _controller.Login(new LoginRequest { Email = "test@example.com", Password = "password" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_ReturnsUnauthorized_WhenPasswordVerificationFails()
        {
            // Arrange
            var mockUser = new User { Email = "test@example.com", PasswordHash = "hashed_password" };
            _mockUserService.Setup(s => s.GetUserByEmail("test@example.com")).Returns(mockUser);
            _mockUserService.Setup(s => s.VerifyHashedPassword("wrong_password", mockUser.PasswordHash)).Returns(false);

            // Act
            var result = _controller.Login(new LoginRequest { Email = "test@example.com", Password = "wrong_password" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

    }
}
