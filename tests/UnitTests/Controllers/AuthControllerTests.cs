using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GymAPI.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;

namespace UnitTests.Controllers
{
	/// <summary>
	/// Unit tests for the AuthController.
	/// </summary>
	public class AuthControllerTests
	{
		private readonly Mock<IJwtService> _jwtServiceMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly AuthController _controller;

		public AuthControllerTests()
		{
			_jwtServiceMock = new Mock<IJwtService>();
			_configurationMock = new Mock<IConfiguration>();

			// Setting environment variables for testing
			Environment.SetEnvironmentVariable("LOGIN_USERNAME", "testUser");
			Environment.SetEnvironmentVariable("LOGIN_PASSWORD", "testPass");

			_controller = new AuthController(_jwtServiceMock.Object, _configurationMock.Object);
		}

		/// <summary>
		/// Tests that valid credentials return OK with a token.
		/// </summary>
		[Fact]
		public void Login_ValidCredentials_ReturnsOkWithToken()
		{
			// Arrange
			var loginModel = new LoginModel
			{
				Username = "testUser",
				Password = "testPass"
			};

			var expectedToken = "fake_jwt_token";
			_jwtServiceMock.Setup(s => s.GenerateJwtToken("testUser"))
						   .Returns(expectedToken);

			// Act
			var result = _controller.Login(loginModel) as OkObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(200);
			result.Value.Should().BeEquivalentTo(new { Token = expectedToken });
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized with "Invalid credentials".
		/// </summary>
		[Fact]
		public void Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var loginModel = new LoginModel
			{
				Username = "wrongUser",
				Password = "wrongPass"
			};

			_jwtServiceMock.Setup(s => s.GenerateJwtToken(It.IsAny<string>()))
						   .Returns((string)null);

			// Act
			var result = _controller.Login(loginModel);

			// Assert
			var unauthorizedResult = result as UnauthorizedObjectResult;
			unauthorizedResult.Should().NotBeNull();
			unauthorizedResult.StatusCode.Should().Be(401);
			unauthorizedResult.Value.Should().Be("Invalid credentials");
		}
	}
}
