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
using GymDBAccess.DTOs;

namespace UnitTests.Services
{
	public class MembershipServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Membership>> _membershipDbSetMock;
		private readonly Mock<DbSet<Member>> _memberDbSetMock;
		private readonly MembershipService _service;

		public MembershipServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_membershipDbSetMock = new Mock<DbSet<Membership>>();
			_dbContextMock.Setup(db => db.Memberships).Returns(_membershipDbSetMock.Object);

			_memberDbSetMock = new Mock<DbSet<Member>>();
			_dbContextMock.Setup(db => db.Members).Returns(_memberDbSetMock.Object);

			_service = new MembershipService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests that GetMembershipByIdAsync returns the membership if found.
		/// </summary>
		[Fact]
		public async Task GetMembershipByIdAsync_WhenFound_ReturnsMembership()
		{
			// Arrange
			var testMembership = new Membership { MembershipID = 1, MemberID = 10 };
			_dbContextMock.Setup(db => db.Memberships.FindAsync(1)).ReturnsAsync(testMembership);

			// Act
			var result = await _service.GetMembershipByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.MembershipID.Should().Be(1);
			result.MemberID.Should().Be(10);
		}

		/// <summary>
		/// Tests that GetMembershipByIdAsync returns null if the membership is not found.
		/// </summary>
		[Fact]
		public async Task GetMembershipByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Memberships.FindAsync(999)).ReturnsAsync((Membership)null);

			// Act
			var result = await _service.GetMembershipByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllMembershipsAsync returns all memberships in the DbSet.
		/// </summary>
		[Fact]
		public async Task GetAllMembershipsAsync_ReturnsAll()
		{
			// Arrange
			var membershipsData = new List<Membership>
			{
				new Membership { MembershipID = 1 },
				new Membership { MembershipID = 2 }
			}.AsQueryable();

			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Provider).Returns(membershipsData.Provider);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Expression).Returns(membershipsData.Expression);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.ElementType).Returns(membershipsData.ElementType);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.GetEnumerator()).Returns(membershipsData.GetEnumerator());

			// Act
			var result = await _service.GetAllMembershipsAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that AddMembershipAsync adds a new membership and saves changes.
		/// </summary>
		[Fact]
		public async Task AddMembershipAsync_AddsAndSaves()
		{
			// Arrange
			var newMembership = new Membership { MembershipID = 10 };

			// Act
			await _service.AddMembershipAsync(newMembership);

			// Assert
			_dbContextMock.Verify(db => db.Memberships.AddAsync(newMembership, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that UpdateMembershipAsync updates the membership and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateMembershipAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingMembership = new Membership { MembershipID = 5 };

			// Act
			await _service.UpdateMembershipAsync(existingMembership);

			// Assert
			_dbContextMock.Verify(db => db.Memberships.Update(existingMembership), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteMembershipAsnyc removes the membership if found.
		/// </summary>
		[Fact]
		public async Task DeleteMembershipAsnyc_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingMembership = new Membership { MembershipID = 7 };
			_dbContextMock.Setup(db => db.Memberships.FindAsync(7)).ReturnsAsync(existingMembership);

			// Act
			await _service.DeleteMembershipAsnyc(7);

			// Assert
			_dbContextMock.Verify(db => db.Memberships.Remove(existingMembership), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteMembershipAsnyc does nothing if the membership is not found.
		/// </summary>
		[Fact]
		public async Task DeleteMembershipAsnyc_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Memberships.FindAsync(999)).ReturnsAsync((Membership)null);

			// Act
			await _service.DeleteMembershipAsnyc(999);

			// Assert
			_dbContextMock.Verify(db => db.Memberships.Remove(It.IsAny<Membership>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		/// <summary>
		/// Tests that GetActiveMembershipsAsync returns only memberships marked as active, joined with Member info.
		/// </summary>
		[Fact]
		public async Task GetActiveMembershipsAsync_ReturnsActiveList()
		{
			// Arrange
			var membershipData = new List<Membership>
			{
				new Membership { MembershipID = 1, MemberID = 10, IsActive = true, PaymentType = "Monthly" },
				new Membership { MembershipID = 2, MemberID = 11, IsActive = false }
			}.AsQueryable();

			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Provider).Returns(membershipData.Provider);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Expression).Returns(membershipData.Expression);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.ElementType).Returns(membershipData.ElementType);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.GetEnumerator()).Returns(membershipData.GetEnumerator());

			var memberData = new List<Member>
			{
				new Member { MemberID = 10, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 11, FirstName = "John", LastName = "Doe" }
			}.AsQueryable();

			var memberDbSetMock = new Mock<DbSet<Member>>();
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(memberData.Provider);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(memberData.Expression);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(memberData.ElementType);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(memberData.GetEnumerator());

			_dbContextMock.Setup(db => db.Members).Returns(memberDbSetMock.Object);

			// Act
			var result = await _service.GetActiveMembershipsAsync();

			// Assert
			result.Should().HaveCount(1);
			result.First().MembershipID.Should().Be(1);
			result.First().MemberName.Should().Be("Alice Smith");
			result.First().Type.Should().Be("Monthly");
		}

		/// <summary>
		/// Tests that GetInactiveMembershipsAsync returns only memberships marked as inactive, joined with Member info.
		/// </summary>
		[Fact]
		public async Task GetInactiveMembershipsAsync_ReturnsInactiveList()
		{
			// Arrange
			var membershipData = new List<Membership>
			{
				new Membership { MembershipID = 1, MemberID = 10, IsActive = true },
				new Membership { MembershipID = 2, MemberID = 11, IsActive = false, PaymentType = "One-time" }
			}.AsQueryable();

			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Provider).Returns(membershipData.Provider);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Expression).Returns(membershipData.Expression);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.ElementType).Returns(membershipData.ElementType);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.GetEnumerator()).Returns(membershipData.GetEnumerator());

			var memberData = new List<Member>
			{
				new Member { MemberID = 10, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 11, FirstName = "John", LastName = "Doe" }
			}.AsQueryable();

			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(memberData.Provider);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(memberData.Expression);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(memberData.ElementType);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(memberData.GetEnumerator());

			// Act
			var result = await _service.GetInactiveMembershipsAsync();

			// Assert
			result.Should().HaveCount(1);
			result.First().MembershipID.Should().Be(2);
			result.First().MemberName.Should().Be("John Doe");
		}

		/// <summary>
		/// Tests that GetUserMembershipsAsync returns memberships for a specific member.
		/// </summary>
		[Fact]
		public async Task GetUserMembershipsAsync_ReturnsUserMemberships()
		{
			// Arrange
			var membershipData = new List<Membership>
			{
				new Membership { MembershipID = 10, MemberID = 100, PaymentType = "Monthly" },
				new Membership { MembershipID = 11, MemberID = 101, PaymentType = "One-time" },
				new Membership { MembershipID = 12, MemberID = 100, PaymentType = "Annual" }
			}.AsQueryable();

			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Provider).Returns(membershipData.Provider);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.Expression).Returns(membershipData.Expression);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.ElementType).Returns(membershipData.ElementType);
			_membershipDbSetMock.As<IQueryable<Membership>>().Setup(m => m.GetEnumerator()).Returns(membershipData.GetEnumerator());

			// Act
			var result = await _service.GetUserMembershipsAsync(100);

			// Assert
			result.Should().HaveCount(2);
			result.First().PaymentType.Should().Be("Monthly");
			result.Last().PaymentType.Should().Be("Annual");
		}
	}
}
