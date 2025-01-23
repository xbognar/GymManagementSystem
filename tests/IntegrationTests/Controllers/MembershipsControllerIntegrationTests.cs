using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using GymDBAccess.Models;
using System.Collections.Generic;
using GymDBAccess.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Controllers
{
	/// <summary>
	/// Integration tests for the MembershipsController endpoints.
	/// Ensures each test runs with a fresh and consistent database state.
	/// </summary>
	public class MembershipsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
	{
		private readonly HttpClient _client;
		private readonly IntegrationTestFixture _fixture;

		public MembershipsControllerIntegrationTests(IntegrationTestFixture fixture)
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
		/// Tests retrieving all memberships after login. We expect 2 seeded (#201, #202).
		/// </summary>
		[Fact]
		public async Task GetAllMemberships_ReturnsOkWithList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var memberships = await response.Content.ReadFromJsonAsync<List<Membership>>();
			memberships.Should().NotBeNull();
			memberships.Count.Should().Be(2, "we seeded 2 memberships (#201, #202)");
		}

		/// <summary>
		/// Tests retrieving a specific membership by ID returns Ok if it exists (#201).
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships/201");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var membership = await response.Content.ReadFromJsonAsync<Membership>();
			membership.Should().NotBeNull();
			membership.MembershipID.Should().Be(201);
		}

		/// <summary>
		/// Tests retrieving a non-existing membership returns NotFound (e.g. #9999).
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenNotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships/9999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a membership returns Created, then verifying retrieval 
		/// by the newly assigned ID (no hard-coded ID).
		/// </summary>
		[Fact]
		public async Task AddMembership_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newMembership = new Membership
			{
				MemberID = 101,
				StartDate = System.DateTime.UtcNow,
				EndDate = System.DateTime.UtcNow.AddDays(30),
				PaymentType = "TestIntegration",
				IsActive = true
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/memberships", newMembership);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var created = await response.Content.ReadFromJsonAsync<Membership>();
			created.Should().NotBeNull();
			created.MembershipID.Should().BeGreaterThan(0, "the DB should assign an auto-generated ID");

			var fetch = await _client.GetAsync($"/api/memberships/{created.MembershipID}");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating a membership returns NoContent. We update seeded #201.
		/// </summary>
		[Fact]
		public async Task UpdateMembership_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var membershipToUpdate = new Membership
			{
				MembershipID = 201,
				MemberID = 101,
				PaymentType = "UpdatedType",
				IsActive = true
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/memberships/201", membershipToUpdate);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests retrieving active memberships returns the seeded active membership(s) (#201).
		/// </summary>
		[Fact]
		public async Task GetActiveMemberships_ReturnsOkAndActiveList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships/active");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var activeList = await response.Content.ReadFromJsonAsync<List<ActiveMembershipDTO>>();
			activeList.Should().NotBeNull();
			activeList.Count.Should().Be(1, "membership #201 is seeded as active");
		}

		/// <summary>
		/// Tests retrieving memberships for a specific user returns Ok if found, 
		/// otherwise NotFound. #101 => membership #201 in seed data.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships/user/101/memberships");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var userMemberships = await response.Content.ReadFromJsonAsync<List<UserMembershipsDTO>>();
			userMemberships.Should().NotBeNull();
			userMemberships.Count.Should().Be(1, "only membership #201 belongs to #101");
		}

		/// <summary>
		/// Tests GetInactiveMemberships returns only inactive memberships from the seed (#202).
		/// </summary>
		[Fact]
		public async Task GetInactiveMemberships_ReturnsOkAndInactiveList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/memberships/inactive");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var inactiveList = await response.Content.ReadFromJsonAsync<List<InactiveMembershipDTO>>();
			inactiveList.Should().HaveCount(1, "membership #202 is seeded as inactive");
		}
	}
}
