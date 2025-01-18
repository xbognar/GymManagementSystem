using System;
using Xunit;
using FluentAssertions;
using Moq;
using GymDBAccess.Services;
using GymDBAccess.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace UnitTests.Services
{
	public class JwtServiceTests
	{
		private readonly JwtService _jwtService;
		private readonly Mock<IConfiguration> _configurationMock;

		public JwtServiceTests()
		{
			_configurationMock = new Mock<IConfiguration>();
			// Example environment variable approach:
			Environment.SetEnvironmentVariable("JWT_KEY", "super_test_jwt_key_for_unit_tests");
			_jwtService = new JwtService();
		}

		/// <summary>
		/// Tests that GenerateJwtToken creates a valid JWT with the expected username claim.
		/// </summary>
		[Fact]
		public void GenerateJwtToken_ValidUsername_ReturnsToken()
		{
			// Arrange
			var username = "testUser";

			// Act
			var token = _jwtService.GenerateJwtToken(username);

			// Assert
			token.Should().NotBeNullOrEmpty();

			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);
			jwtToken.Claims.Any(c => c.Type == "unique_name" && c.Value == username).Should().BeTrue();
		}

		/// <summary>
		/// Tests that GenerateJwtToken sets an expiration claim (default is 1 hour).
		/// </summary>
		[Fact]
		public void GenerateJwtToken_HasExpirationWithinOneHour()
		{
			// Arrange
			var username = "someUser";

			// Act
			var token = _jwtService.GenerateJwtToken(username);
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);

			// Assert
			jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), precision: TimeSpan.FromMinutes(5));
		}

		/// <summary>
		/// Tests that if the environment variable JWT_KEY is not set, 
		/// the generated token might be invalid or cause an error.
		/// (Here we only demonstrate if we want a negative scenario.)
		/// </summary>
		[Fact]
		public void GenerateJwtToken_WhenJwtKeyNotSet_ThrowsOrGeneratesToken()
		{
			// Arrange
			// Save the current key, then unset
			var originalKey = Environment.GetEnvironmentVariable("JWT_KEY");
			Environment.SetEnvironmentVariable("JWT_KEY", null);

			var serviceNoKey = new JwtService();

			// Act & Assert
			try
			{
				var token = serviceNoKey.GenerateJwtToken("test");
				token.Should().NotBeNullOrEmpty("Service might still produce a token but it's insecure!");
			}
			finally
			{
				// Restore environment variable
				Environment.SetEnvironmentVariable("JWT_KEY", originalKey);
			}
		}
	}
}
