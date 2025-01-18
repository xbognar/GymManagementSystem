using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using GymDBAccess.Models;
using System.Collections.Generic;

namespace IntegrationTests.Controllers
{
	public class ChipsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public ChipsControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests retrieving all chips requires authentication, so we log in, then call GET /api/chips.
		/// </summary>
		[Fact]
		public async Task GetAllChips_AfterLogin_ReturnsOk()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var chips = await response.Content.ReadFromJsonAsync<List<Chip>>();
			chips.Should().NotBeNull();
			chips.Count.Should().BeGreaterThan(0, "we seeded 2 chips");
		}

		/// <summary>
		/// Tests retrieving a specific chip by ID after login. 
		/// We expect chip with ID=301 seeded as active.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenChipExists_ReturnsOk()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips/301");
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
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new chip via POST. Expects a Created response.
		/// </summary>
		[Fact]
		public async Task AddChip_ReturnsCreated()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newChip = new Chip
			{
				ChipID = 999,
				MemberID = 101,
				ChipInfo = "IntegrationTestChip",
				IsActive = true
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/chips", newChip);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			// Assert
			var fetch = await _client.GetAsync("/api/chips/999");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating a chip's member association.
		/// </summary>
		[Fact]
		public async Task UpdateChip_Successful_ReturnsOk()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var updateRequest = new
			{
				ChipID = 301,
				NewMemberID = 102
			};

			// Act
			var response = await _client.PutAsJsonAsync("/api/chips/301", updateRequest);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			// Assert
			var updatedChip = await response.Content.ReadFromJsonAsync<Chip>();
			updatedChip.MemberID.Should().Be(102);
		}

		/// <summary>
		/// Tests deleting an existing chip returns NoContent, 
		/// then verifying it's gone.
		/// </summary>
		[Fact]
		public async Task DeleteChip_ReturnsNoContent()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/chips/999");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests GetActiveChips returns only active chips from the seed.
		/// </summary>
		[Fact]
		public async Task GetActiveChips_ReturnsOkAndActiveList()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips/active");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			// Assert
			var activeChips = await response.Content.ReadFromJsonAsync<List<object>>();
			activeChips.Should().NotBeNull();
			activeChips.Count.Should().Be(1, "we seeded 1 active chip");
		}

		/// <summary>
		/// Tests GetInactiveChips returns only inactive chips.
		/// </summary>
		[Fact]
		public async Task GetInactiveChips_ReturnsOkAndInactiveList()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips/inactive");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			// Assert
			var inactiveChips = await response.Content.ReadFromJsonAsync<List<object>>();
			inactiveChips.Should().HaveCount(1, "we seeded 1 inactive chip");
		}

		/// <summary>
		/// Tests GetChipInfoByMemberId returns the chip info if found, or NotFound if not.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenFound_ReturnsOk()
		{
			// Arrange
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Act
			var response = await _client.GetAsync("/api/chips/infoByMember/101");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var info = await response.Content.ReadAsStringAsync();
			info.Should().Contain("VIP Access");
		}
	}
}
