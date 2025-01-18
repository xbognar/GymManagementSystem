using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using GymDBAccess.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Controllers
{
	public class AuthControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public AuthControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests that valid credentials return an OK response with a Token property.
		/// </summary>
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkAndToken()
		{
			// Act
			var loginModel = new LoginModel
			{
				Username = System.Environment.GetEnvironmentVariable("LOGIN_USERNAME") ?? "testUser",
				Password = System.Environment.GetEnvironmentVariable("LOGIN_PASSWORD") ?? "testPass"
			};

			var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var json = await response.Content.ReadFromJsonAsync<dynamic>();
			json.Token.Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized with "Invalid credentials".
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// Arrange
			var loginModel = new LoginModel
			{
				Username = "wrongUser",
				Password = "wrongPass"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

			var message = await response.Content.ReadAsStringAsync();
			message.Should().Contain("Invalid credentials");
		}
	}
}
