using Xunit;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.DTOs;
using GymDBAccess.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class MembershipServiceTests : IDisposable
{
	private readonly ApplicationDbContext _context;
	private readonly MembershipService _membershipService;

	public MembershipServiceTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
			.Options;

		_context = new ApplicationDbContext(options);
		_membershipService = new MembershipService(_context);

		_context.Members.Add(new Member
		{
			MemberID = 1,
			FirstName = "John",
			LastName = "Doe",
			Email = "john.doe@example.com", 
			PhoneNumber = "1234567890" 
		});
		_context.Members.Add(new Member
		{
			MemberID = 2,
			FirstName = "Jane",
			LastName = "Doe",
			Email = "jane.doe@example.com", 
			PhoneNumber = "0987654321" 
		});

		_context.Memberships.Add(new Membership
		{
			MembershipID = 1,
			MemberID = 1,
			StartDate = DateTime.Now.AddMonths(-6),
			EndDate = DateTime.Now.AddMonths(6),
			Type = "Annual",
			IsActive = true
		});
		_context.Memberships.Add(new Membership
		{
			MembershipID = 2,
			MemberID = 2,
			StartDate = DateTime.Now.AddMonths(-12),
			EndDate = DateTime.Now.AddMonths(-1),
			Type = "Annual",
			IsActive = false
		});

		_context.SaveChanges();
	}

	[Fact]
	public async Task GetMembershipByIdAsync_ReturnsMembership()
	{
		// Act
		var result = await _membershipService.GetMembershipByIdAsync(1);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Annual", result.Type);
		Assert.True(result.IsActive);
	}

	[Fact]
	public async Task GetAllMembershipsAsync_ReturnsAllMemberships()
	{
		// Act
		var result = await _membershipService.GetAllMembershipsAsync();

		// Assert
		var memberships = result.ToList();
		Assert.Equal(2, memberships.Count);
	}

	[Fact]
	public async Task AddMembershipAsync_AddsMembership()
	{
		// Arrange
		var newMembership = new Membership { MembershipID = 3, MemberID = 3, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1), Type = "Monthly", IsActive = true };

		// Act
		await _membershipService.AddMembershipAsync(newMembership);
		var addedMembership = await _context.Memberships.FindAsync(3);

		// Assert
		Assert.NotNull(addedMembership);
		Assert.Equal("Monthly", addedMembership.Type);
		Assert.True(addedMembership.IsActive);
	}

	[Fact]
	public async Task UpdateMembershipAsync_UpdatesMembership()
	{
		// Arrange
		var membershipToUpdate = await _context.Memberships.FindAsync(1);
		membershipToUpdate.Type = "Monthly";

		// Act
		await _membershipService.UpdateMembershipAsync(membershipToUpdate);
		var updatedMembership = await _context.Memberships.FindAsync(1);

		// Assert
		Assert.NotNull(updatedMembership);
		Assert.Equal("Monthly", updatedMembership.Type);
	}

	[Fact]
	public async Task DeleteMembershipAsync_DeletesMembership()
	{
		// Act
		await _membershipService.DeleteMembershipAsnyc(2);
		var deletedMembership = await _context.Memberships.FindAsync(2);

		// Assert
		Assert.Null(deletedMembership);
	}

	[Fact]
	public async Task GetActiveMembershipsAsync_ReturnsOnlyActiveMemberships()
	{
		// Act
		var result = await _membershipService.GetActiveMembershipsAsync();

		// Assert
		var activeMemberships = result.ToList();
		Assert.NotEmpty(activeMemberships);
		Assert.All(activeMemberships, dto => Assert.True(dto.EndDate > DateTime.Now));
		Assert.Equal("John Doe", activeMemberships.First().MemberName);
	}

	[Fact]
	public async Task GetInactiveMembershipsAsync_ReturnsOnlyInactiveMemberships()
	{
		// Act
		var result = await _membershipService.GetInactiveMembershipsAsync();

		// Assert
		var inactiveMemberships = result.ToList();
		Assert.NotEmpty(inactiveMemberships);
		Assert.All(inactiveMemberships, dto => Assert.True(dto.EndDate <= DateTime.Now));
		Assert.Equal("Jane Doe", inactiveMemberships.First().MemberName);
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}
}
