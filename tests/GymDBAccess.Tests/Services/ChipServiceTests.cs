﻿using Xunit;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ChipServiceTests : IDisposable
{
	private readonly ApplicationDbContext _context;
	private readonly ChipService _chipService;

	public ChipServiceTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test run
			.Options;

		_context = new ApplicationDbContext(options);
		_chipService = new ChipService(_context);

		// Seed the database with some test chips
		_context.Chips.Add(new Chip { ChipID = 1, MemberID = 1, IssueDate = DateTime.Now.AddDays(-10), IsActive = true });
		_context.Chips.Add(new Chip { ChipID = 2, MemberID = 2, IssueDate = DateTime.Now.AddDays(-5), IsActive = false });
		_context.SaveChanges();
	}

	[Fact]
	public async Task GetChipByIdAsync_ReturnsChip()
	{
		// Act
		var result = await _chipService.GetChipByIdAsync(1);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsActive);
		Assert.Equal(1, result.MemberID);
	}

	[Fact]
	public async Task GetAllChipsAsync_ReturnsAllChips()
	{
		// Act
		var result = await _chipService.GetAllChipsAsync();

		// Assert
		var chips = result.ToList();
		Assert.Equal(2, chips.Count);
	}

	[Fact]
	public async Task AddChipAsync_AddsChip()
	{
		// Arrange
		var newChip = new Chip { ChipID = 3, MemberID = 3, IssueDate = DateTime.Now, IsActive = true };

		// Act
		await _chipService.AddChipAsync(newChip);
		var addedChip = await _context.Chips.FindAsync(3);

		// Assert
		Assert.NotNull(addedChip);
		Assert.True(addedChip.IsActive);
		Assert.Equal(3, addedChip.MemberID);
	}

	[Fact]
	public async Task UpdateChipAsync_UpdatesChip()
	{
		// Arrange
		var chipToUpdate = await _context.Chips.FindAsync(1);
		chipToUpdate.IsActive = false;

		// Act
		await _chipService.UpdateChipAsync(chipToUpdate);
		var updatedChip = await _context.Chips.FindAsync(1);

		// Assert
		Assert.NotNull(updatedChip);
		Assert.False(updatedChip.IsActive);
	}

	[Fact]
	public async Task DeleteChipAsync_DeletesChip()
	{
		// Act
		await _chipService.DeleteChipAsync(2);
		var deletedChip = await _context.Chips.FindAsync(2);

		// Assert
		Assert.Null(deletedChip);
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}
}