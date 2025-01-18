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
	public class MembersControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private readonly HttpClient _client;

		public MembersControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_client = fixture.CreateClient();
		}

		/// <summary>
		/// Tests getting all members after logging in. Expects seeded members.
		/// </summary>
		[Fact]
		public async Task GetAllMembers_ReturnsOkWithList()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/members");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var members = await response.Content.ReadFromJsonAsync<List<Member>>();
			members.Should().NotBeNull();
			members.Count.Should().BeGreaterThan(0);
		}

		/// <summary>
		/// Tests retrieving a specific member by ID returns Ok if it exists.
		/// </summary>
		[Fact]
		public async Task GetMember_WhenFound_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/members/101");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var member = await response.Content.ReadFromJsonAsync<Member>();
			member.Should().NotBeNull();
			member.MemberID.Should().Be(101);
		}

		/// <summary>
		/// Tests retrieving a non-existent member returns NotFound.
		/// </summary>
		[Fact]
		public async Task GetMember_WhenNotFound_ReturnsNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var response = await _client.GetAsync("/api/members/9999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests creating a new member returns CreatedAtAction, then verifying retrieval.
		/// </summary>
		[Fact]
		public async Task AddMember_ReturnsCreated()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var newMember = new Member
			{
				MemberID = 999,
				FirstName = "Testy",
				LastName = "McTestFace"
			};

			var response = await _client.PostAsJsonAsync("/api/members", newMember);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			// Optional: fetch it back
			var fetch = await _client.GetAsync("/api/members/999");
			fetch.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		/// <summary>
		/// Tests updating an existing member returns NoContent if IDs match.
		/// </summary>
		[Fact]
		public async Task UpdateMember_ReturnsNoContent()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// We'll update seeded member #101
			var updated = new Member
			{
				MemberID = 101,
				FirstName = "Alice",
				LastName = "Updated"
			};

			var response = await _client.PutAsJsonAsync("/api/members/101", updated);
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Tests that mismatched IDs return BadRequest.
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMismatch_ReturnsBadRequest()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			var mismatch = new Member
			{
				MemberID = 999,
				FirstName = "Mismatch"
			};

			var response = await _client.PutAsJsonAsync("/api/members/101", mismatch);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		/// <summary>
		/// Tests deleting an existing member returns NoContent, otherwise NotFound.
		/// </summary>
		[Fact]
		public async Task DeleteMember_ReturnsNoContentOrNotFound()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// We'll try to delete #999 if we created it above
			var response = await _client.DeleteAsync("/api/members/999");

			response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Tests retrieving the ID of a member by name if it exists.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenFound_ReturnsOk()
		{
			var token = await TestUtilities.LoginAndGetTokenAsync(_client);
			_client.AddAuthToken(token);

			// Our seeded #101 is "Alice Smith"
			var response = await _client.GetAsync("/api/members/getMemberIdByName?fullName=Alice Smith");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var id = await response.Content.ReadFromJsonAsync<int>();
			id.Should().Be(101);
		}
	}
}
