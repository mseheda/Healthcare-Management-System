using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [TestMethod]
        public void GetUserByEmail_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var mockUser = new User { Email = "test@example.com", PasswordHash = "hashed_password" };
            _mockUserRepository.Setup(r => r.GetUserByEmail("test@example.com")).Returns(mockUser);

            // Act
            var result = _userService.GetUserByEmail("test@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result?.Email);
        }

        [TestMethod]
        public void GetUserByEmail_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetUserByEmail("test@example.com")).Returns((User?)null);

            // Act
            var result = _userService.GetUserByEmail("test@example.com");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void VerifyHashedPassword_ReturnsFalse_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var plainPassword = "wrong_password";
            var hashedPassword = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd8825f6f6c57ffbe58"; // SHA256 hash of "password"

            // Act
            var result = _userService.VerifyHashedPassword(plainPassword, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
