using MinimalTaskControl.Core.Models;
using MinimalTaskControl.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalTaskControl.Infrastructure.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                SecretKey = "SuperSecretKeyForTestingMinimum32CharsLong!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 60
            };

            _jwtService = new JwtService(_jwtSettings);
        }

        [Fact]
        public void GenerateToken_WithUsername_ReturnsValidToken()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token = _jwtService.GenerateToken(username);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
            Assert.Equal(_jwtSettings.Audience, jwtToken.Audiences.First());
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == username);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }

        [Fact]
        public void GenerateToken_WithUsernameAndRole_ReturnsTokenWithCorrectRole()
        {
            // Arrange
            var username = "adminuser";
            var role = "Admin";

            // Act
            var token = _jwtService.GenerateToken(username, role);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
        }

        [Fact]
        public void GenerateToken_WithEmptyRole_CreatesRoleClaimWithEmptyValue()
        {
            // Arrange
            var username = "testuser";
            var emptyRole = "";

            // Act
            var token = _jwtService.GenerateToken(username, emptyRole);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Assert.NotNull(roleClaim);
            Assert.Equal("", roleClaim.Value); // Пустая строка, не "User"
        }

        [Fact]
        public void GenerateToken_WithEmptyUsername_CreatesNameClaimWithEmptyValue()
        {
            // Arrange
            var emptyUsername = "";

            // Act
            var token = _jwtService.GenerateToken(emptyUsername);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            Assert.NotNull(nameClaim);
            Assert.Equal("", nameClaim.Value);
        }

        [Fact]
        public void GenerateToken_WithNullRole_ThrowsArgumentNullException()
        {
            // Arrange
            var username = "testuser";
            string nullRole = default!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(username, nullRole));
        }

        [Fact]
        public void GenerateToken_WithNullUsername_ThrowsArgumentNullException()
        {
            // Arrange
            string nullUsername = default!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(nullUsername));
        }

        [Fact]
        public void GenerateToken_WithDifferentUsernames_ReturnsDifferentTokens()
        {
            // Arrange
            var user1 = "user1";
            var user2 = "user2";

            // Act
            var token1 = _jwtService.GenerateToken(user1);
            var token2 = _jwtService.GenerateToken(user2);

            // Assert
            Assert.NotEqual(token1, token2);

            var handler = new JwtSecurityTokenHandler();
            var claims1 = handler.ReadJwtToken(token1).Claims;
            var claims2 = handler.ReadJwtToken(token2).Claims;

            Assert.Contains(claims1, c => c.Value == user1);
            Assert.Contains(claims2, c => c.Value == user2);
        }

        [Fact]
        public void GenerateToken_ContainsExpirationDate()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token = _jwtService.GenerateToken(username);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
            Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes + 1));
        }

        [Fact]
        public void GenerateToken_WithCustomRole_ReturnsTokenWithCustomRole()
        {
            // Arrange
            var username = "manager";
            var customRole = "Manager";

            // Act
            var token = _jwtService.GenerateToken(username, customRole);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == customRole);
        }

        [Fact]
        public void GenerateToken_ContainsJtiClaim()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token = _jwtService.GenerateToken(username);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            Assert.NotNull(jtiClaim);
            Assert.True(Guid.TryParse(jtiClaim.Value, out _));
        }

        [Fact]
        public void GenerateToken_WithDifferentCalls_GeneratesDifferentJti()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token1 = _jwtService.GenerateToken(username);
            var token2 = _jwtService.GenerateToken(username);

            // Assert
            var handler = new JwtSecurityTokenHandler();

            var jti1 = handler.ReadJwtToken(token1).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var jti2 = handler.ReadJwtToken(token2).Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            Assert.NotEqual(jti1, jti2);
        }
    }
}
