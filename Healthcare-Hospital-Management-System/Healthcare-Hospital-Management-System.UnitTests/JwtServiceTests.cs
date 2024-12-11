using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class JwtServiceTests
    {
        private IJwtService _jwtService;
        private IOptions<JwtOptions> _jwtOptions;

        [TestInitialize]
        public void Setup()
        {
            // Simulate configuration values
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("JwtOptions:Key", "supersecurekeyfromtest1234567890"),
                    new KeyValuePair<string, string>("JwtOptions:Issuer", "testIssuer"),
                    new KeyValuePair<string, string>("JwtOptions:Audience", "testAudience"),
                    new KeyValuePair<string, string>("JwtOptions:ExpiresInMinutes", "60")
                })
                .Build();

            var options = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            _jwtOptions = Options.Create(options);

            _jwtService = new JwtService(_jwtOptions);
        }

        [TestMethod]
        public void GenerateToken_ReturnsValidJwt()
        {
            // Arrange
            string username = "testUser";

            // Act
            string token = _jwtService.GenerateToken(username);

            // Assert
            Assert.IsNotNull(token, "Token should not be null.");
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.IsTrue(tokenHandler.CanReadToken(token), "Generated token should be readable.");
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityTokenMalformedException))]
        public void ValidateToken_ThrowsException_WhenTokenIsInvalid()
        {
            string invalidToken = "invalidToken";
            _jwtService.ValidateToken(invalidToken);
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityTokenExpiredException))]
        public void ValidateToken_ThrowsException_WhenTokenIsExpired()
        {
            var expiredOptions = Options.Create(new JwtOptions
            {
                Key = "supersecurekeythatmeetstherequirements",
                Issuer = "testIssuer",
                Audience = "testAudience",
                ExpiresInMinutes = -1
            });

            var jwtService = new JwtService(expiredOptions);
            string token = jwtService.GenerateToken("testUser");
            jwtService.ValidateToken(token);
        }

        [TestMethod]
        public void ValidateToken_ReturnsValidClaimsPrincipal_WhenTokenIsValid()
        {
            string username = "testUser";
            string token = _jwtService.GenerateToken(username);

            ClaimsPrincipal claimsPrincipal = _jwtService.ValidateToken(token);

            Assert.IsNotNull(claimsPrincipal);

            var claim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);
            Assert.IsNotNull(claim, "Subject claim should exist.");
            Assert.AreEqual(username, claim.Value, "Subject claim value should match the username.");
        }
    }
}
