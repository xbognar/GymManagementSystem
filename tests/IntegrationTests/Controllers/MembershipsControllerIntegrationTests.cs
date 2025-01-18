using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.Dependencies;
using GymDBAccess.Models;
using System.Collections.Generic;
using GymDBAccess.DTOs;

namespace IntegrationTests.Controllers
{
	public class MembershipsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public MembershipsControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests retrieving all memberships after login.
		/// </summary>
		[Fact]
		public async Task GetAllMemberships_ReturnsOkWithList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/memberships");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var memberships = await response.Content.ReadFromJsonAsync<List<Membership>>();
			memberships.Should().NotBeNull();
			memberships.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Tests retrieving a specific membership by ID returns Ok if it exists.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenFound_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// We seeded #201
			var response = await _client.GetAsync("/api/memberships/201");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var membership = await response.Content.ReadFromJsonAsync<Membership>();
			membership.Should().NotBeNull();
			membership.MembershipID.Should().Be(201);
		}

		/// <summary>
		/// Tests retrieving a non-existing membership returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenNotFound_ReturnsNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/memberships/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a membership returns Created, then verifying retrieval.
		/// </summary>
		[Fact]
		public async Task AddMembership_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newMembership = new Membership
			{
				MembershipID = 999,
				MemberID = 101, // seeded Alice
				StartDate = System.DateTime.UtcNow,
				EndDate = System.DateTime.UtcNow.AddDays(30),
				PaymentType = "TestIntegration",
				IsActive = true
			};

			var response = await _client.PostAsJsonAsync("/api/memberships", newMembership);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			// fetch it back
			var fetch = await _client.GetAsync("/api/memberships/999");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating a membership returns NoContent.
		/// </summary>
		[Fact]
		public async Task UpdateMembership_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var membershipToUpdate = new Membership
			{
				MembershipID = 201, // seeded
				MemberID = 101,
				PaymentType = "UpdatedType",
				IsActive = true
			};

			var response = await _client.PutAsJsonAsync("/api/memberships/201", membershipToUpdate);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests deleting a membership that exists returns NoContent, else NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteMembership_ReturnsNoContentOrNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.DeleteAsync("/api/memberships/999");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests retrieving active memberships returns a list containing the seeded active membership(s).
		/// </summary>
		[Fact]
		public async Task GetActiveMemberships_ReturnsOkAndActiveList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/memberships/active");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var activeList = await response.Content.ReadFromJsonAsync<List<ActiveMembershipDTO>>();
			activeList.Should().NotBeNull();
			activeList.Count.Should().Be(1); // seeded #201 is active
		}

		/// <summary>
		/// Tests retrieving inactive memberships returns the one(s) we seeded as inactive.
		/// </summary>
		[Fact]
		public async Task GetInactiveMemberships_ReturnsOkAndInactiveList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/memberships/inactive");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var inactiveList = await response.Content.ReadFromJsonAsync<List<InactiveMembershipDTO>>();
			inactiveList.Should().HaveCount(1); // membership #202
		}

		/// <summary>
		/// Tests retrieving memberships for a specific user returns Ok if found, otherwise NotFound.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenFound_ReturnsOk()
		{
			// #101 => membership #201
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/memberships/user/101/memberships");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var userMemberships = await response.Content.ReadFromJsonAsync<List<UserMembershipsDTO>>();
			userMemberships.Should().NotBeNull();
			userMemberships.Count.Should().Be(1);
		}
	}
}
