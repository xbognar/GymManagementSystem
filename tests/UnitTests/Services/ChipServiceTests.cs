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
	public class ChipServiceTests
	{
		private readonly Mock<ApplicationDbContext> _dbContextMock;
		private readonly Mock<DbSet<Chip>> _chipDbSetMock;
		private readonly Mock<DbSet<Member>> _memberDbSetMock;
		private readonly ChipService _service;

		public ChipServiceTests()
		{
			var options = new DbContextOptions<ApplicationDbContext>();
			_dbContextMock = new Mock<ApplicationDbContext>(options);

			_chipDbSetMock = new Mock<DbSet<Chip>>();
			_dbContextMock.Setup(db => db.Chips).Returns(_chipDbSetMock.Object);

			_memberDbSetMock = new Mock<DbSet<Member>>();
			_dbContextMock.Setup(db => db.Members).Returns(_memberDbSetMock.Object);

			_service = new ChipService(_dbContextMock.Object);
		}

		/// <summary>
		/// Tests that GetChipByIdAsync returns the chip if found.
		/// </summary>
		[Fact]
		public async Task GetChipByIdAsync_WhenFound_ReturnsChip()
		{
			// Arrange
			var testChip = new Chip { ChipID = 1, MemberID = 50 };
			_dbContextMock.Setup(db => db.Chips.FindAsync(1)).ReturnsAsync(testChip);

			// Act
			var result = await _service.GetChipByIdAsync(1);

			// Assert
			result.Should().NotBeNull();
			result.ChipID.Should().Be(1);
		}

		/// <summary>
		/// Tests that GetChipByIdAsync returns null when the chip is not found.
		/// </summary>
		[Fact]
		public async Task GetChipByIdAsync_WhenNotFound_ReturnsNull()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Chips.FindAsync(999)).ReturnsAsync((Chip)null);

			// Act
			var result = await _service.GetChipByIdAsync(999);

			// Assert
			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that GetAllChipsAsync returns all chips from the DbSet.
		/// </summary>
		[Fact]
		public async Task GetAllChipsAsync_ReturnsAllChips()
		{
			// Arrange
			var chipsData = new List<Chip>
			{
				new Chip { ChipID = 1 },
				new Chip { ChipID = 2 }
			}.AsQueryable();

			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Provider).Returns(chipsData.Provider);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Expression).Returns(chipsData.Expression);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.ElementType).Returns(chipsData.ElementType);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.GetEnumerator()).Returns(chipsData.GetEnumerator());

			// Act
			var result = await _service.GetAllChipsAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		/// <summary>
		/// Tests that AddChipAsync adds a new chip and saves changes.
		/// </summary>
		[Fact]
		public async Task AddChipAsync_AddsAndSaves()
		{
			// Arrange
			var newChip = new Chip { ChipID = 10 };

			// Act
			await _service.AddChipAsync(newChip);

			// Assert
			_dbContextMock.Verify(db => db.Chips.AddAsync(newChip, default), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that UpdateChipAsync updates a chip and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateChipAsync_UpdatesAndSaves()
		{
			// Arrange
			var existingChip = new Chip { ChipID = 5 };

			// Act
			await _service.UpdateChipAsync(existingChip);

			// Assert
			_dbContextMock.Verify(db => db.Chips.Update(existingChip), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteChipAsync removes the chip if found.
		/// </summary>
		[Fact]
		public async Task DeleteChipAsync_WhenFound_DeletesAndSaves()
		{
			// Arrange
			var existingChip = new Chip { ChipID = 7 };
			_dbContextMock.Setup(db => db.Chips.FindAsync(7)).ReturnsAsync(existingChip);

			// Act
			await _service.DeleteChipAsync(7);

			// Assert
			_dbContextMock.Verify(db => db.Chips.Remove(existingChip), Times.Once);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
		}

		/// <summary>
		/// Tests that DeleteChipAsync does nothing if the chip is not found.
		/// </summary>
		[Fact]
		public async Task DeleteChipAsync_WhenNotFound_DoesNothing()
		{
			// Arrange
			_dbContextMock.Setup(db => db.Chips.FindAsync(999)).ReturnsAsync((Chip)null);

			// Act
			await _service.DeleteChipAsync(999);

			// Assert
			_dbContextMock.Verify(db => db.Chips.Remove(It.IsAny<Chip>()), Times.Never);
			_dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
		}

		/// <summary>
		/// Tests that GetActiveChipsAsync returns only active chips with joined member info.
		/// </summary>
		[Fact]
		public async Task GetActiveChipsAsync_ReturnsActiveChipDTOs()
		{
			// Arrange
			var chipsData = new List<Chip>
			{
				new Chip { ChipID = 1, MemberID = 10, IsActive = true, ChipInfo = "Info1" },
				new Chip { ChipID = 2, MemberID = 11, IsActive = false, ChipInfo = "Info2" }
			}.AsQueryable();

			var chipDbSetMock = new Mock<DbSet<Chip>>();
			chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Provider).Returns(chipsData.Provider);
			chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Expression).Returns(chipsData.Expression);
			chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.ElementType).Returns(chipsData.ElementType);
			chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.GetEnumerator()).Returns(chipsData.GetEnumerator());

			_dbContextMock.Setup(db => db.Chips).Returns(chipDbSetMock.Object);

			var membersData = new List<Member>
			{
				new Member { MemberID = 10, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 11, FirstName = "John", LastName = "Doe" }
			}.AsQueryable();

			var memberDbSetMock = new Mock<DbSet<Member>>();
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(membersData.Provider);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(membersData.Expression);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(membersData.ElementType);
			memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(membersData.GetEnumerator());

			_dbContextMock.Setup(db => db.Members).Returns(memberDbSetMock.Object);

			// Act
			var result = await _service.GetActiveChipsAsync();

			// Assert
			result.Should().HaveCount(1);
			var first = result.First();
			first.ChipID.Should().Be(1);
			first.ChipInfo.Should().Be("Info1");
			first.OwnerFullName.Should().Be("Alice Smith");
		}

		/// <summary>
		/// Tests that GetInactiveChipsAsync returns only inactive chips with joined member info.
		/// </summary>
		[Fact]
		public async Task GetInactiveChipsAsync_ReturnsInactiveChipDTOs()
		{
			// Arrange
			var chipsData = new List<Chip>
			{
				new Chip { ChipID = 1, MemberID = 10, IsActive = true, ChipInfo = "ActiveChip" },
				new Chip { ChipID = 2, MemberID = 11, IsActive = false, ChipInfo = "InactiveChip" }
			}.AsQueryable();

			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Provider).Returns(chipsData.Provider);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Expression).Returns(chipsData.Expression);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.ElementType).Returns(chipsData.ElementType);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.GetEnumerator()).Returns(chipsData.GetEnumerator());

			var membersData = new List<Member>
			{
				new Member { MemberID = 10, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 11, FirstName = "John", LastName = "Doe" }
			}.AsQueryable();

			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(membersData.Provider);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(membersData.Expression);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(membersData.ElementType);
			_memberDbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(membersData.GetEnumerator());

			// Act
			var result = await _service.GetInactiveChipsAsync();

			// Assert
			result.Should().HaveCount(1);
			result.First().ChipInfo.Should().Be("InactiveChip");
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberIdAsync returns the ChipInfo for the given member ID if it exists.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberIdAsync_WhenChipExists_ReturnsChipInfo()
		{
			// Arrange
			var chipsData = new List<Chip>
			{
				new Chip { ChipID = 5, MemberID = 100, ChipInfo = "VIP Access", IsActive = true }
			}.AsQueryable();

			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Provider).Returns(chipsData.Provider);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Expression).Returns(chipsData.Expression);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.ElementType).Returns(chipsData.ElementType);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.GetEnumerator()).Returns(chipsData.GetEnumerator());

			// Act
			var result = await _service.GetChipInfoByMemberIdAsync(100);

			// Assert
			result.Should().Be("VIP Access");
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberIdAsync returns null if no chip matches the given member ID.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberIdAsync_WhenNoChipFound_ReturnsNull()
		{
			// Arrange
			var chipsData = new List<Chip>
			{
				new Chip { ChipID = 5, MemberID = 101, ChipInfo = "OtherUserChip", IsActive = true }
			}.AsQueryable();

			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Provider).Returns(chipsData.Provider);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.Expression).Returns(chipsData.Expression);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.ElementType).Returns(chipsData.ElementType);
			_chipDbSetMock.As<IQueryable<Chip>>().Setup(m => m.GetEnumerator()).Returns(chipsData.GetEnumerator());

			// Act
			var result = await _service.GetChipInfoByMemberIdAsync(999);

			// Assert
			result.Should().BeNull();
		}
	}
}
