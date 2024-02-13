using Xunit;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class MemberServiceTests : IDisposable
{
	private readonly ApplicationDbContext _context;
	private readonly MemberService _memberService;

	public MemberServiceTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
			.Options;

		_context = new ApplicationDbContext(options);
		_memberService = new MemberService(_context);

		_context.Members.Add(new Member { MemberID = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1), Email = "john.doe@example.com", PhoneNumber = "1234567890" });
		_context.Members.Add(new Member { MemberID = 2, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1985, 2, 2), Email = "jane.doe@example.com", PhoneNumber = "0987654321" });
		_context.SaveChanges();
	}

	[Fact]
	public async Task GetMemberByIdAsync_ReturnsMember()
	{
		// Act
		var result = await _memberService.GetMemberByIdAsync(1);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("John", result.FirstName);
		Assert.Equal("Doe", result.LastName);
	}

	[Fact]
	public async Task GetAllMembersAsync_ReturnsAllMembers()
	{
		// Act
		var result = await _memberService.GetAllMembersAsync();

		// Assert
		var members = result.ToList();
		Assert.Equal(2, members.Count);
	}

	[Fact]
	public async Task AddMemberAsync_AddsMember()
	{
		// Arrange
		var newMember = new Member { MemberID = 3, FirstName = "Jim", LastName = "Beam", DateOfBirth = new DateTime(1990, 3, 3), Email = "jim.beam@example.com", PhoneNumber = "5551234567" };

		// Act
		await _memberService.AddMemberAsync(newMember);
		var addedMember = await _context.Members.FindAsync(3);

		// Assert
		Assert.NotNull(addedMember);
		Assert.Equal("Jim", addedMember.FirstName);
		Assert.Equal("Beam", addedMember.LastName);
	}

	[Fact]
	public async Task UpdateMemberAsync_UpdatesMember()
	{
		// Arrange
		var memberToUpdate = await _context.Members.FindAsync(1);
		memberToUpdate.FirstName = "UpdatedName";

		// Act
		await _memberService.UpdateMemberAsync(memberToUpdate);
		var updatedMember = await _context.Members.FindAsync(1);

		// Assert
		Assert.NotNull(updatedMember);
		Assert.Equal("UpdatedName", updatedMember.FirstName);
	}

	[Fact]
	public async Task DeleteMemberAsync_DeletesMember()
	{
		// Act
		await _memberService.DeleteMemberAsync(2);
		var deletedMember = await _context.Members.FindAsync(2);

		// Assert
		Assert.Null(deletedMember);
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}
}
