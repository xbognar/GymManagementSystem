using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using GymDBAccess.Models;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for the AuthController endpoints.
	/// Ensures each test runs with a fresh and consistent database state.
	/// </summary>
	public class AuthControllerIntegrationTests : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
	{
		private readonly HttpClient _client;
		private readonly IntegrationTestFixture _fixture;

		public AuthControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_fixture = fixture;
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Resets and reseeds the database before each test.
		/// </summary>
		public async Task InitializeAsync()
		{
			using var scope = _fixture.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<GymDBAccess.DataAccess.ApplicationDbContext>();

			// Clear existing data 
			db.Chips.RemoveRange(db.Chips);
			db.Memberships.RemoveRange(db.Memberships);
			db.Members.RemoveRange(db.Members);
			await db.SaveChangesAsync();

			// Reseed the database
			IntegrationTests.Dependencies.SeedDataHelper.Seed(db);
		}

		public Task DisposeAsync() => Task.CompletedTask;

		/// <summary>
		/// Tests that valid credentials return an OK response with a Token property.
		/// </summary>
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkAndToken()
		{
			// ARRANGE
			var loginModel = new LoginModel
			{
				Username = "testUser",
				Password = "testPass"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			// Read as a dictionary
			var dict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
			dict.Should().NotBeNull();
			dict.Should().ContainKey("token");
			var token = dict["token"];
			token.Should().NotBeNullOrEmpty();
		}

		/// <summary>
		/// Tests that invalid credentials return Unauthorized with "Invalid credentials".
		/// </summary>
		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorized()
		{
			// ARRANGE
			var loginModel = new LoginModel
			{
				Username = "wrongUser",
				Password = "wrongPass"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
			var message = await response.Content.ReadAsStringAsync();
			message.Should().Contain("Invalid credentials");
		}
	}
}
