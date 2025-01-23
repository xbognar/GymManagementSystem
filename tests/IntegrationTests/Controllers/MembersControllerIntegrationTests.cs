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
	/// Integration tests for the MembersController endpoints.
	/// Ensures each test runs with a fresh and consistent database state.
	/// </summary>
	public class MembersControllerIntegrationTests : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
	{
		private readonly HttpClient _client;
		private readonly IntegrationTestFixture _fixture;

		public MembersControllerIntegrationTests(IntegrationTestFixture fixture)
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
		/// Tests getting all members after logging in. Expects seeded members (#101, #102).
		/// </summary>
		[Fact]
		public async Task GetAllMembers_ReturnsOkWithList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/members");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var members = await response.Content.ReadFromJsonAsync<List<Member>>();
			members.Should().NotBeNull();
			members.Count.Should().Be(2, "SeedDataHelper seeds 2 members (#101, #102)");
		}

		/// <summary>
		/// Tests retrieving a specific member by ID returns Ok if it exists (#101).
		/// </summary>
		[Fact]
		public async Task GetMember_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/members/101");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var member = await response.Content.ReadFromJsonAsync<Member>();
			member.Should().NotBeNull();
			member.MemberID.Should().Be(101);
		}

		/// <summary>
		/// Tests retrieving a non-existent member returns NotFound (e.g. #9999).
		/// </summary>
		[Fact]
		public async Task GetMember_WhenNotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/members/9999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new member returns CreatedAtAction, 
		/// then verifying retrieval by the new auto-generated ID.
		/// </summary>
		[Fact]
		public async Task AddMember_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newMember = new Member
			{
				FirstName = "Testy",
				LastName = "McTestFace"
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/members", newMember);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var created = await response.Content.ReadFromJsonAsync<Member>();
			created.Should().NotBeNull();
			created.MemberID.Should().BeGreaterThan(0, "the DB should assign a new ID");

			var fetch = await _client.GetAsync($"/api/members/{created.MemberID}");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating an existing seeded member (#101) returns NoContent if IDs match.
		/// </summary>
		[Fact]
		public async Task UpdateMember_ReturnsNoContent()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var updated = new Member
			{
				MemberID = 101,
				FirstName = "Alice",
				LastName = "Updated"
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/members/101", updated);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests that mismatched IDs return BadRequest 
		/// (e.g. passing MemberID=999 but calling /api/members/101).
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMismatch_ReturnsBadRequest()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var mismatch = new Member
			{
				MemberID = 999,
				FirstName = "Mismatch"
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/members/101", mismatch);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests deleting an existing or non-existing member returns NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteMember_ReturnsNoContentOrNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/members/999");

			// ASSERT
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests retrieving the ID of a member by name if it exists ("Alice Smith" => #101).
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/members/getMemberIdByName?fullName=Alice Smith");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var id = await response.Content.ReadFromJsonAsync<int>();
			id.Should().Be(101);
		}
	}
}
