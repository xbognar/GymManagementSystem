using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services;
using GymDBAccess.Services.Interfaces;

namespace UnitTests.Services
{
	/// <summary>
	/// Unit tests for the <see cref="MemberService"/> class, ensuring correct behavior of member-related operations.
	/// </summary>
	public class MemberServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly MemberService _service;
		private readonly string _databaseName;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemberServiceTests"/> class.
		/// Sets up the in-memory database and seeds initial data.
		/// </summary>
		public MemberServiceTests()
		{
			_databaseName = Guid.NewGuid().ToString();

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: _databaseName)
				.Options;

			_dbContext = new ApplicationDbContext(options);

			_dbContext.Members.AddRange(new List<Member>
			{
				new Member { MemberID = 1, FirstName = "John", LastName = "Doe" },
				new Member { MemberID = 2, FirstName = "Jane", LastName = "Doe" }
			});

			_dbContext.SaveChanges();

			_service = new MemberService(_dbContext);
		}

		/// <summary>
		/// Tests that <see cref="MemberService.GetMemberByIdAsync(int)"/> returns the member when it exists.
		/// </summary>
		[Fact]
		public async Task GetMemberByIdAsync_ShouldReturnMember_WhenMemberExists()
		{
			int memberId = 1;

			var result = await _service.GetMemberByIdAsync(memberId);

			result.Should().NotBeNull();
			result.MemberID.Should().Be(memberId);
			result.FirstName.Should().Be("John");
			result.LastName.Should().Be("Doe");
		}

		/// <summary>
		/// Tests that <see cref="MemberService.GetMemberByIdAsync(int)"/> returns null when the member does not exist.
		/// </summary>
		[Fact]
		public async Task GetMemberByIdAsync_ShouldReturnNull_WhenMemberDoesNotExist()
		{
			int memberId = 999;

			var result = await _service.GetMemberByIdAsync(memberId);

			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MemberService.GetAllMembersAsync"/> returns all members.
		/// </summary>
		[Fact]
		public async Task GetAllMembersAsync_ShouldReturnAllMembers()
		{
			var result = await _service.GetAllMembersAsync();

			result.Should().HaveCount(2);
			result.Should().Contain(m => m.MemberID == 1 && m.FirstName == "John" && m.LastName == "Doe");
			result.Should().Contain(m => m.MemberID == 2 && m.FirstName == "Jane" && m.LastName == "Doe");
		}

		/// <summary>
		/// Tests that <see cref="MemberService.AddMemberAsync(Member)"/> adds a new member and saves changes.
		/// </summary>
		[Fact]
		public async Task AddMemberAsync_ShouldAddMember_AndSaveChanges()
		{
			var newMember = new Member { MemberID = 10, FirstName = "Test", LastName = "User" };

			await _service.AddMemberAsync(newMember);

			var memberInDb = await _dbContext.Members.FindAsync(newMember.MemberID);
			memberInDb.Should().NotBeNull();
			memberInDb.FirstName.Should().Be("Test");
			memberInDb.LastName.Should().Be("User");
		}

		/// <summary>
		/// Tests that <see cref="MemberService.UpdateMemberAsync(Member)"/> updates an existing member and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateMemberAsync_ShouldUpdateMember_AndSaveChanges()
		{
			var existingMember = await _dbContext.Members.FindAsync(1);
			existingMember.FirstName = "Johnny";

			await _service.UpdateMemberAsync(existingMember);

			var memberInDb = await _dbContext.Members.FindAsync(existingMember.MemberID);
			memberInDb.FirstName.Should().Be("Johnny");
		}

		/// <summary>
		/// Tests that <see cref="MemberService.DeleteMemberAsync(int)"/> removes the member when it exists and saves changes.
		/// </summary>
		[Fact]
		public async Task DeleteMemberAsync_ShouldRemoveMember_AndSaveChanges_WhenMemberExists()
		{
			int memberId = 2;

			await _service.DeleteMemberAsync(memberId);

			var memberInDb = await _dbContext.Members.FindAsync(memberId);
			memberInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MemberService.DeleteMemberAsync(int)"/> does nothing when the member does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteMemberAsync_ShouldDoNothing_WhenMemberDoesNotExist()
		{
			int memberId = 999;

			await _service.DeleteMemberAsync(memberId);

			var memberInDb = await _dbContext.Members.FindAsync(memberId);
			memberInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MemberService.GetMemberIdByNameAsync(string)"/> returns the MemberID when the name matches.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByNameAsync_ShouldReturnMemberId_WhenNameMatches()
		{
			string fullName = "Alice Smith";
			var newMember = new Member { MemberID = 50, FirstName = "Alice", LastName = "Smith" };
			_dbContext.Members.Add(newMember);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetMemberIdByNameAsync(fullName);

			result.Should().Be(50);
		}

		/// <summary>
		/// Tests that <see cref="MemberService.GetMemberIdByNameAsync(string)"/> returns null when the name does not match or is in a bad format.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByNameAsync_ShouldReturnNull_WhenNameDoesNotMatch_Or_BadFormat()
		{
			string badFormat = "JustOneName";
			string noMatch = "Unknown Person";

			var badFormatResult = await _service.GetMemberIdByNameAsync(badFormat);
			var noMatchResult = await _service.GetMemberIdByNameAsync(noMatch);

			badFormatResult.Should().BeNull();
			noMatchResult.Should().BeNull();
		}

		/// <summary>
		/// Disposes the in-memory database after all tests are run.
		/// </summary>
		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
