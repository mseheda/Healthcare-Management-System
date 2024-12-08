using Healthcare_Hospital_Management_System.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DataProtectServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private DataProtectService _dataProtectService;

        [TestInitialize]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _dataProtectService = new DataProtectService(_mockConfiguration.Object);
        }

        [TestMethod]
        public void ExportPublicKey_ReturnsValidKey()
        {
            // Act
            string publicKey = _dataProtectService.ExportPublicKey();

            // Assert
            Assert.IsNotNull(publicKey, "Public key should not be null.");
            Assert.IsTrue(Convert.FromBase64String(publicKey).Length > 0, "Public key should be a valid Base64 string.");
        }

        [TestMethod]
        public void ImportPublicKey_DoesNotThrowException_WithValidKey()
        {
            // Arrange
            string publicKey = _dataProtectService.ExportPublicKey();

            // Act & Assert
            try
            {
                _dataProtectService.ImportPublicKey(publicKey);
            }
            catch (Exception ex)
            {
                Assert.Fail($"ImportPublicKey threw an unexpected exception: {ex.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ImportPrivateKeyFromSecrets_ThrowsException_WhenKeyIsMissing()
        {
            // Arrange
            _mockConfiguration.Setup(config => config["EncryptionKeys:PrivateKey"]).Returns((string)null);

            // Act
            _dataProtectService.ImportPrivateKeyFromSecrets();
        }

        [TestMethod]
        public async Task EncryptAsync_ReturnsEncryptedData()
        {
            // Arrange
            string publicKey = _dataProtectService.ExportPublicKey();
            string plainText = "Test encryption message";

            // Act
            byte[] encryptedData = await _dataProtectService.EncryptAsync(publicKey, plainText, CancellationToken.None);

            // Assert
            Assert.IsNotNull(encryptedData, "Encrypted data should not be null.");
            Assert.IsTrue(encryptedData.Length > 0, "Encrypted data should not be empty.");
        }
    }
}
