using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GymAPI.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System;

namespace UnitTests.Controllers
{
	public class AuthControllerTests
	{
		private readonly Mock<IJwtService> _jwtServiceMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly AuthController _controller;

		public AuthControllerTests()
		{
			_jwtServiceMock = new Mock<IJwtService>();
			_configurationMock = new Mock<IConfiguration>();

			// You might set environment variables or mock them here:
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
				Username = "testUser", // Matches environment variable
				Password = "testPass"
			};

			_jwtServiceMock.Setup(s => s.GenerateJwtToken("testUser"))
						   .Returns("fake_jwt_token");

			// Act
			var result = _controller.Login(loginModel) as OkObjectResult;

			// Assert
			result.Should().NotBeNull();
			var tokenObject = result.Value as dynamic;
			tokenObject.Token.Should().Be("fake_jwt_token");
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized.
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

			// Act
			var result = _controller.Login(loginModel);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
			unauthorizedResult.Value.Should().Be("Invalid credentials");
		}
	}
}
