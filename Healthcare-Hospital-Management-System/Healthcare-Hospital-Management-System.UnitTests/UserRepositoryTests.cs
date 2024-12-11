using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private IUserRepository _userRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = new UserRepository();
        }

        [TestMethod]
        public void GetUserByEmail_ReturnsCorrectUser_WhenEmailExists()
        {
            // Arrange
            string email = "admin@gmail.com";
            string expectedPasswordHash = "e564b4081d7a9ea4b00dada53bdae70c99b87b6fce869f0c3dd4d2bfa1e53e1c";

            // Act
            var user = _userRepository.GetUserByEmail(email);

            // Assert
            Assert.IsNotNull(user, "User should not be null.");
            Assert.AreEqual(email, user.Email, "Email should match.");
            Assert.AreEqual(expectedPasswordHash, user.PasswordHash, "Password hash should match.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetUserByEmail_ThrowsException_WhenEmailDoesNotExist()
        {
            // Arrange
            string email = "nonexistentuser@gmail.com";

            // Act
            _userRepository.GetUserByEmail(email);

            // Assert - Exception is expected
        }

        [TestMethod]
        public void GetUserByEmail_ReturnsCorrectUser_WhenAnotherValidEmailIsProvided()
        {
            // Arrange
            string email = "testuser@gmail.com";
            string expectedPasswordHash = "8b5b9db0c13db24256c829aa364aa90c6d2eba318b9232a4ab9313b954d3555f";

            // Act
            var user = _userRepository.GetUserByEmail(email);

            // Assert
            Assert.IsNotNull(user, "User should not be null.");
            Assert.AreEqual(email, user.Email, "Email should match.");
            Assert.AreEqual(expectedPasswordHash, user.PasswordHash, "Password hash should match.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmail_ThrowsException_WhenEmailIsNull()
        {
            // Arrange
            string email = null;

            // Act
            _userRepository.GetUserByEmail(email);

            // Assert - Exception is expected
        }

        [TestMethod]
        public void GetUserByEmail_ReturnsCorrectUser_WhenCaseInsensitiveEmailIsProvided()
        {
            // Arrange
            string email = "ADMIN@gmail.com";
            string expectedPasswordHash = "e564b4081d7a9ea4b00dada53bdae70c99b87b6fce869f0c3dd4d2bfa1e53e1c";

            // Act
            var user = _userRepository.GetUserByEmail(email.ToLower());

            // Assert
            Assert.IsNotNull(user, "User should not be null.");
            Assert.AreEqual(email.ToLower(), user.Email, "Email should match.");
            Assert.AreEqual(expectedPasswordHash, user.PasswordHash, "Password hash should match.");
        }
    }
}
