using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services;
using GymDBAccess.Services.Interfaces;

namespace UnitTests.Services
{
	public class MemberServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Member>> _memberDbSetMock;
		private readonly MemberService _service;

		public MemberServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_memberDbSetMock = new Mock<DbSet<Member>>();
			_dbContextMock.Setup(db => db.Members).Returns(_memberDbSetMock.Object);

			_service = new MemberService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests that GetMemberByIdAsync returns the member if found.
		/// </summary>
		[Fact]
		public async Task GetMemberByIdAsync_WhenFound_ReturnsMember()
		{
			// Arrange
			var testMember = new Member { MemberID = 1, FirstName = "John", LastName = "Doe" };
			_dbContextMock.Setup(db => db.Members.FindAsync(1)).ReturnsAsync(testMember);

			// Act
			var result = await _service.GetMemberByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.MemberID.Should().Be(1);
			result.FirstName.Should().Be("John");
		}

		/// <summary>
		/// Tests that GetMemberByIdAsync returns null if the member is not found.
		/// </summary>
		[Fact]
		public async Task GetMemberByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Members.FindAsync(999)).ReturnsAsync((Member)null);

			// Act
			var result = await _service.GetMemberByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllMembersAsync returns all the members in the database.
		/// </summary>
		[Fact]
		public async Task GetAllMembersAsync_ReturnsAllMembers()
		{
			// Arrange
			var membersData = new List<Member>
			{
				new Member { MemberID = 1 },
				new Member { MemberID = 2 }
			}.AsQueryable();

			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(membersData.Provider);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(membersData.Expression);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(membersData.ElementType);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(membersData.GetEnumerator());

			// Act
			var result = await _service.GetAllMembersAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that AddMemberAsync adds a new member and saves changes.
		/// </summary>
		[Fact]
		public async Task AddMemberAsync_AddsAndSaves()
		{
			// Arrange
			var newMember = new Member { MemberID = 10 };

			// Act
			await _service.AddMemberAsync(newMember);

			// Assert
			_dbContextMock.Verify(db => db.Members.AddAsync(newMember, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that UpdateMemberAsync updates a member and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateMemberAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingMember = new Member { MemberID = 5 };

			// Act
			await _service.UpdateMemberAsync(existingMember);

			// Assert
			_dbContextMock.Verify(db => db.Members.Update(existingMember), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteMemberAsync removes a member when found, then saves.
		/// </summary>
		[Fact]
		public async Task DeleteMemberAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingMember = new Member { MemberID = 7 };
			_dbContextMock.Setup(db => db.Members.FindAsync(7)).ReturnsAsync(existingMember);

			// Act
			await _service.DeleteMemberAsync(7);

			// Assert
			_dbContextMock.Verify(db => db.Members.Remove(existingMember), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteMemberAsync does nothing if the member is not found.
		/// </summary>
		[Fact]
		public async Task DeleteMemberAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Members.FindAsync(999)).ReturnsAsync((Member)null);

			// Act
			await _service.DeleteMemberAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.Members.Remove(It.IsAny<Member>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		/// <summary>
		/// Tests that GetMemberIdByNameAsync returns the MemberID if the name matches.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByNameAsync_WhenMatchFound_ReturnsMemberID()
		{
			// Arrange
			var membersData = new List<Member>
			{
				new Member { MemberID = 50, FirstName = "Alice", LastName = "Smith" }
			}.AsQueryable();

			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(membersData.Provider);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(membersData.Expression);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(membersData.ElementType);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(membersData.GetEnumerator());

			// Act
			var result = await _service.GetMemberIdByNameAsync("Alice Smith");

			// Assert
			result.Should().Be(50);
		}

		/// <summary>
		/// Tests that GetMemberIdByNameAsync returns null if the format is incorrect or no match found.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByNameAsync_WhenNoMatchOrBadFormat_ReturnsNull()
		{
			// Arrange
			var membersData = new List<Member>
			{
				new Member { MemberID = 51, FirstName = "Some", LastName = "One" }
			}.AsQueryable();

			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(membersData.Provider);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(membersData.Expression);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(membersData.ElementType);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(membersData.GetEnumerator());

			// Act
			var badFormat = await _service.GetMemberIdByNameAsync("JustOneName");
			var noMatch = await _service.GetMemberIdByNameAsync("Unknown Person");

			// Assert
			badFormat.Should().BeNull();
			noMatch.Should().BeNull();
		}
	}
}
