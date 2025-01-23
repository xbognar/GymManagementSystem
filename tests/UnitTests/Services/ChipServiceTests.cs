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
	/// Unit tests for the <see cref="ChipService"/> class, ensuring correct behavior of chip-related operations.
	/// </summary>
	public class ChipServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ChipService _service;
		private readonly string _databaseName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChipServiceTests"/> class.
		/// Sets up the in-memory database and seeds initial data.
		/// </summary>
		public ChipServiceTests()
		{
			_databaseName = Guid.NewGuid().ToString();

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: _databaseName)
				.Options;

			_dbContext = new ApplicationDbContext(options);

			_dbContext.Chips.AddRange(new List<Chip>
			{
				new Chip { ChipID = 1, MemberID = 50 },
				new Chip { ChipID = 2, MemberID = 51 }
			});

			_dbContext.SaveChanges();

			_service = new ChipService(_dbContext);
		}

		/// <summary>
		/// Tests that <see cref="ChipService.GetChipByIdAsync(int)"/> returns the chip when it exists.
		/// </summary>
		[Fact]
		public async Task GetChipByIdAsync_ShouldReturnChip_WhenChipExists()
		{
			int chipId = 1;

			var result = await _service.GetChipByIdAsync(chipId);

			result.Should().NotBeNull();
			result.ChipID.Should().Be(chipId);
			result.MemberID.Should().Be(50);
		}

		/// <summary>
		/// Tests that <see cref="ChipService.GetChipByIdAsync(int)"/> returns null when the chip does not exist.
		/// </summary>
		[Fact]
		public async Task GetChipByIdAsync_ShouldReturnNull_WhenChipDoesNotExist()
		{
			int chipId = 999;

			var result = await _service.GetChipByIdAsync(chipId);

			result.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="ChipService.GetAllChipsAsync"/> returns all chips.
		/// </summary>
		[Fact]
		public async Task GetAllChipsAsync_ShouldReturnAllChips()
		{
			var result = await _service.GetAllChipsAsync();

			result.Should().HaveCount(2);
			result.Select(c => c.ChipID).Should().Contain(new List<int> { 1, 2 });
		}

		/// <summary>
		/// Tests that <see cref="ChipService.AddChipAsync(Chip)"/> adds a new chip and saves changes.
		/// </summary>
		[Fact]
		public async Task AddChipAsync_ShouldAddChip_AndSaveChanges()
		{
			var newChip = new Chip { ChipID = 10, MemberID = 60 };

			await _service.AddChipAsync(newChip);

			var chipInDb = await _dbContext.Chips.FindAsync(newChip.ChipID);
			chipInDb.Should().NotBeNull();
			chipInDb.ChipID.Should().Be(10);
			chipInDb.MemberID.Should().Be(60);
		}

		/// <summary>
		/// Tests that <see cref="ChipService.UpdateChipAsync(Chip)"/> updates an existing chip and saves changes.
		/// </summary>
		[Fact]
		public async Task UpdateChipAsync_ShouldUpdateChip_AndSaveChanges()
		{
			var existingChip = await _dbContext.Chips.FindAsync(1);
			existingChip.ChipInfo = "Updated Info";

			await _service.UpdateChipAsync(existingChip);

			var chipInDb = await _dbContext.Chips.FindAsync(existingChip.ChipID);
			chipInDb.ChipInfo.Should().Be("Updated Info");
		}

		/// <summary>
		/// Tests that <see cref="ChipService.DeleteChipAsync(int)"/> removes the chip when it exists and saves changes.
		/// </summary>
		[Fact]
		public async Task DeleteChipAsync_ShouldRemoveChip_AndSaveChanges_WhenChipExists()
		{
			int chipId = 2;

			await _service.DeleteChipAsync(chipId);

			var chipInDb = await _dbContext.Chips.FindAsync(chipId);
			chipInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="ChipService.DeleteChipAsync(int)"/> does nothing when the chip does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteChipAsync_ShouldDoNothing_WhenChipDoesNotExist()
		{
			int chipId = 999;

			await _service.DeleteChipAsync(chipId);

			var chipInDb = await _dbContext.Chips.FindAsync(chipId);
			chipInDb.Should().BeNull();
		}

		/// <summary>
		/// Tests that <see cref="ChipService.GetChipInfoByMemberIdAsync(int)"/> returns the ChipInfo when the chip exists.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberIdAsync_ShouldReturnChipInfo_WhenChipExists()
		{
			var newChip = new Chip { ChipID = 3, MemberID = 100, ChipInfo = "VIP Access", IsActive = true };
			_dbContext.Chips.Add(newChip);
			await _dbContext.SaveChangesAsync();

			var result = await _service.GetChipInfoByMemberIdAsync(100);

			result.Should().Be("VIP Access");
		}

		/// <summary>
		/// Tests that <see cref="ChipService.GetChipInfoByMemberIdAsync(int)"/> returns null when the chip does not exist.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberIdAsync_ShouldReturnNull_WhenChipDoesNotExist()
		{
			int memberId = 999;

			var result = await _service.GetChipInfoByMemberIdAsync(memberId);

			result.Should().BeNull();
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
