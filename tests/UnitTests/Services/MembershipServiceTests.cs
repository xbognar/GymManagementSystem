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
using GymDBAccess.DTOs;

namespace UnitTests.Services
{
	/// <summary>
	/// Unit tests for the <see cref="MembershipService"/> class, ensuring correct behavior of membership-related operations.
	/// </summary>
	public class MembershipServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly MembershipService _service;
		private readonly string _databaseName;

		/// <summary>
		/// Initializes a new instance of the <see cref="MembershipServiceTests"/> class.
		/// Sets up the in-memory database and seeds initial data.
		/// </summary>
		public MembershipServiceTests()
		{
			_databaseName = Guid.NewGuid().ToString();

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: _databaseName)
				.Options;

			_dbContext = new ApplicationDbContext(options);

			_dbContext.Memberships.AddRange(new List<Membership>
			{
				new Membership { MembershipID = 1, MemberID = 10, PaymentType = "Monthly", IsActive = true },
				new Membership { MembershipID = 2, MemberID = 11, PaymentType = "Annual", IsActive = false }
			});

			_dbContext.Members.AddRange(new List<Member>
			{
				new Member { MemberID = 10, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 11, FirstName = "John", LastName = "Doe" }
			});

			_dbContext.SaveChanges();

			_service = new MembershipService(_dbContext);
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetMembershipByIdAsync(int)"/> returns the membership when it exists.
		/// </summary>
		[Fact]
		public async Task GetMembershipByIdAsync_ShouldReturnMembership_WhenExists()
		{
			int membershipId = 1;

			var result = await _service.GetMembershipByIdAsync(membershipId);

			result.Should().NotBeNull();
			result.MembershipID.Should().Be(membershipId);
			result.MemberID.Should().Be(10);
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetMembershipByIdAsync(int)"/> returns null when the membership does not exist.
		/// </summary>
		[Fact]
		public async Task GetMembershipByIdAsync_ShouldReturnNull_WhenNotExists()
		{
			int membershipId = 999;

			var result = await _service.GetMembershipByIdAsync(membershipId);

			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetAllMembershipsAsync"/> returns all memberships.
		/// </summary>
		[Fact]
		public async Task GetAllMembershipsAsync_ShouldReturnAllMemberships()
		{
			var result = await _service.GetAllMembershipsAsync();

			result.Should().HaveCount(2);
			result.Should().Contain(m => m.MembershipID == 1);
			result.Should().Contain(m => m.MembershipID == 2);
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.AddMembershipAsync(Membership)"/> adds a new membership and saves changes.
		/// </summary>
		[Fact]
		public async Task AddMembershipAsync_ShouldAddMembership_AndSaveChanges()
		{
			var newMembership = new Membership { MembershipID = 10, MemberID = 100, PaymentType = "Monthly", IsActive = true };

			await _service.AddMembershipAsync(newMembership);

			var membershipInDb = await _dbContext.Memberships.FindAsync(newMembership.MembershipID);
			membershipInDb.Should().NotBeNull();
			membershipInDb.PaymentType.Should().Be("Monthly");
			membershipInDb.IsActive.Should().BeTrue();
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.UpdateMembershipAsync(Membership)"/> updates an existing membership and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateMembershipAsync_ShouldUpdateMembership_AndSaveChanges()
		{
			var existingMembership = await _dbContext.Memberships.FindAsync(1);
			existingMembership.PaymentType = "Weekly";

			await _service.UpdateMembershipAsync(existingMembership);

			var membershipInDb = await _dbContext.Memberships.FindAsync(existingMembership.MembershipID);
			membershipInDb.PaymentType.Should().Be("Weekly");
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.DeleteMembershipAsync(int)"/> removes the membership when it exists and saves changes.
		/// </summary>
		[Fact]
		public async Task DeleteMembershipAsync_ShouldRemoveMembership_AndSaveChanges_WhenExists()
		{
			int membershipId = 2;

			await _service.DeleteMembershipAsnyc(membershipId);

			var membershipInDb = await _dbContext.Memberships.FindAsync(membershipId);
			membershipInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.DeleteMembershipAsync(int)"/> does nothing when the membership does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteMembershipAsync_ShouldDoNothing_WhenNotExists()
		{
			int membershipId = 999;

			await _service.DeleteMembershipAsnyc(membershipId);

			var membershipInDb = await _dbContext.Memberships.FindAsync(membershipId);
			membershipInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetActiveMembershipsAsync"/> returns only active memberships with member information.
		/// </summary>
		[Fact]
		public async Task GetActiveMembershipsAsync_ShouldReturnActiveMemberships_WithMemberInfo()
		{
			var result = await _service.GetActiveMembershipsAsync();

			result.Should().HaveCount(1);
			var activeMembership = result.First();
			activeMembership.MembershipID.Should().Be(1);
			activeMembership.MemberName.Should().Be("Alice Smith");
			activeMembership.Type.Should().Be("Monthly");
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetInactiveMembershipsAsync"/> returns only inactive memberships with member information.
		/// </summary>
		[Fact]
		public async Task GetInactiveMembershipsAsync_ShouldReturnInactiveMemberships_WithMemberInfo()
		{
			var result = await _service.GetInactiveMembershipsAsync();

			result.Should().HaveCount(1);
			var inactiveMembership = result.First();
			inactiveMembership.MembershipID.Should().Be(2);
			inactiveMembership.MemberName.Should().Be("John Doe");
			inactiveMembership.Type.Should().Be("Annual");
		}

		/// <summary>
		/// Tests that <see cref="MembershipService.GetUserMembershipsAsync(int)"/> returns memberships for a specific member.
		/// </summary>
		[Fact]
		public async Task GetUserMembershipsAsync_ShouldReturnMemberships_ForSpecificMember()
		{
			var newMembership1 = new Membership { MembershipID = 10, MemberID = 100, PaymentType = "Monthly", IsActive = true };
			var newMembership2 = new Membership { MembershipID = 11, MemberID = 100, PaymentType = "Annual", IsActive = false };
			_dbContext.Memberships.AddRange(newMembership1, newMembership2);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetUserMembershipsAsync(100);

			result.Should().HaveCount(2);
			result.Should().Contain(m => m.MembershipID == 10 && m.PaymentType == "Monthly");
			result.Should().Contain(m => m.MembershipID == 11 && m.PaymentType == "Annual");
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
