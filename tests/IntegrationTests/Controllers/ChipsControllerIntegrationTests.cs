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
	/// Integration tests for the ChipsController endpoints.
	/// Ensures each test runs with a fresh and consistent database state.
	/// </summary>
	public class ChipsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
	{
		private readonly HttpClient _client;
		private readonly IntegrationTestFixture _fixture;

		public ChipsControllerIntegrationTests(IntegrationTestFixture fixture)
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
		/// Tests retrieving all chips requires authentication, 
		/// so we log in, then call GET /api/chips.
		/// </summary>
		[Fact]
		public async Task GetAllChips_AfterLogin_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var chips = await response.Content.ReadFromJsonAsync<List<Chip>>();
			chips.Should().NotBeNull();
			chips.Count.Should().Be(2, "we seeded 2 chips (IDs 301 & 302)");
		}

		/// <summary>
		/// Tests retrieving a specific chip by ID after login (ID=301 seeded).
		/// </summary>
		[Fact]
		public async Task GetChip_WhenChipExists_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips/301");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var chip = await response.Content.ReadFromJsonAsync<Chip>();
			chip.Should().NotBeNull();
			chip.ChipID.Should().Be(301);
		}

		/// <summary>
		/// Tests that requesting a non-existing chip returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenNotFound_ReturnsNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips/9999");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new chip via POST, letting the DB generate an ID. 
		/// Expects a Created response, then verifies we can GET it by the new ID.
		/// </summary>
		[Fact]
		public async Task AddChip_ReturnsCreated()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newChip = new Chip
			{
				MemberID = 101,
				ChipInfo = "IntegrationTestChip",
				IsActive = true
			};

			// ACT
			var response = await _client.PostAsJsonAsync("/api/chips", newChip);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var createdChip = await response.Content.ReadFromJsonAsync<Chip>();
			createdChip.Should().NotBeNull("the response should contain a newly created chip");
			createdChip.ChipID.Should().BeGreaterThan(0, "the DB should assign an auto-generated ID");

			var fetch = await _client.GetAsync($"/api/chips/{createdChip.ChipID}");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK,
				"we should be able to retrieve by the new ID");
		}

		/// <summary>
		/// Tests updating a seeded chip's member association (chip #301 => member #102).
		/// </summary>
		[Fact]
		public async Task UpdateChip_Successful_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var updateRequest = new
			{
				ChipID = 301,
				NewMemberID = 102
			};

			// ACT
			var response = await _client.PutAsJsonAsync("/api/chips/301", updateRequest);

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var updatedChip = await response.Content.ReadFromJsonAsync<Chip>();
			updatedChip.MemberID.Should().Be(102);
		}

		/// <summary>
		/// Tests deleting an existing or non-existing chip returns NoContent or NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteChip_ReturnsNoContentOrNotFound()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.DeleteAsync("/api/chips/999");

			// ASSERT
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests GetActiveChips returns only active chips from the seed (chip #301).
		/// </summary>
		[Fact]
		public async Task GetActiveChips_ReturnsOkAndActiveList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips/active");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var activeChips = await response.Content.ReadFromJsonAsync<List<Chip>>();
			activeChips.Should().NotBeNull();
			activeChips.Count.Should().Be(1, "we seeded 1 active chip (#301)");
		}

		/// <summary>
		/// Tests GetChipInfoByMemberId returns the chip info if found, or NotFound if not.
		/// We expect "VIP Access" for Member #101 from the seed.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenFound_ReturnsOk()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips/infoByMember/101");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var info = await response.Content.ReadAsStringAsync();
			info.Should().Contain("VIP Access");
		}

		/// <summary>
		/// Tests GetInactiveChips returns only inactive chips from the seed (#302).
		/// </summary>
		[Fact]
		public async Task GetInactiveChips_ReturnsOkAndInactiveList()
		{
			// ARRANGE
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// ACT
			var response = await _client.GetAsync("/api/chips/inactive");

			// ASSERT
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var inactiveChips = await response.Content.ReadFromJsonAsync<List<InactiveChipDTO>>();
			inactiveChips.Should().HaveCount(1, "we seeded 1 inactive chip (#302)");
		}
	}
}
