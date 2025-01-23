using System;
using Xunit;
using FluentAssertions;
using Moq;
using GymDBAccess.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace UnitTests.Services
{
	/// <summary>
	/// Unit tests for the <see cref="JwtService"/> class, ensuring correct JWT generation.
	/// </summary>
	public class JwtServiceTests
	{
		private readonly JwtService _jwtService;

		public JwtServiceTests()
		{
			Environment.SetEnvironmentVariable("JWT_KEY", "super_test_jwt_key_for_unit_tests");
			_jwtService = new JwtService();
		}

		/// <summary>
		/// Tests that GenerateJwtToken creates a valid JWT with the expected username claim.
		/// </summary>
		[Fact]
		public void GenerateJwtToken_ValidUsername_ReturnsToken()
		{
			var username = "testUser";

			var token = _jwtService.GenerateJwtToken(username);

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
			var username = "someUser";

			var token = _jwtService.GenerateJwtToken(username);
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);

			jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), precision: TimeSpan.FromMinutes(5));
		}

		/// <summary>
		/// Tests that if the environment variable JWT_KEY is not set, 
		/// the service uses a fallback and still generates a token.
		/// </summary>
		[Fact]
		public void GenerateJwtToken_WhenJwtKeyNotSet_ReturnsToken()
		{
			var originalKey = Environment.GetEnvironmentVariable("JWT_KEY");
			Environment.SetEnvironmentVariable("JWT_KEY", null);
			var serviceNoKey = new JwtService();

			try
			{
				var token = serviceNoKey.GenerateJwtToken("test");
				token.Should().NotBeNullOrEmpty("Service might still produce a fallback token!");
			}
			finally
			{
				Environment.SetEnvironmentVariable("JWT_KEY", originalKey);
			}
		}
	}
}
