using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GymDBAccess.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymAPI.Tests.Controllers
{
	public class ChipsControllerTests
	{
		private readonly Mock<IChipService> _mockChipService;
		private readonly ChipsController _chipsController;

		public ChipsControllerTests()
		{
			_mockChipService = new Mock<IChipService>();
			_chipsController = new ChipsController(_mockChipService.Object);
		}

		[Fact]
		public async Task GetChip_ReturnsNotFound_WhenChipDoesNotExist()
		{
			// Arrange
			_mockChipService.Setup(service => service.GetChipByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Chip)null);

			// Act
			var result = await _chipsController.GetChip(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task GetChip_ReturnsChip_WhenChipExists()
		{
			// Arrange
			var expectedChip = new Chip { ChipID = 1, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = true };
			_mockChipService.Setup(service => service.GetChipByIdAsync(1))
				.ReturnsAsync(expectedChip);

			// Act
			var result = await _chipsController.GetChip(1);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedChip = Assert.IsType<Chip>(okResult.Value);
			Assert.Equal(expectedChip, returnedChip);
		}

		[Fact]
		public async Task GetAllChips_ReturnsAllChips()
		{
			// Arrange
			var expectedChips = new List<Chip>
			{
				new Chip { ChipID = 1, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = true },
				new Chip { ChipID = 2, MemberID = 2, IssueDate = System.DateTime.Now, IsActive = false }
			};
			_mockChipService.Setup(service => service.GetAllChipsAsync())
				.ReturnsAsync(expectedChips);

			// Act
			var result = await _chipsController.GetAllChips();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedChips = Assert.IsType<List<Chip>>(okResult.Value);
			Assert.Equal(expectedChips, returnedChips);
		}

		[Fact]
		public async Task AddChip_ReturnsCreatedAtAction_WhenChipIsAdded()
		{
			// Arrange
			var newChip = new Chip { ChipID = 3, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = true };
			_mockChipService.Setup(service => service.AddChipAsync(newChip))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _chipsController.AddChip(newChip);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedChip = Assert.IsType<Chip>(createdAtActionResult.Value);
			Assert.Equal(newChip, returnedChip);
		}

		[Fact]
		public async Task UpdateChip_ReturnsBadRequest_WhenChipIdDoesNotMatch()
		{
			// Arrange
			var updatedChip = new Chip { ChipID = 1, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = false };

			// Act
			var result = await _chipsController.UpdateChip(2, updatedChip);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task UpdateChip_ReturnsNoContent_WhenChipIsUpdated()
		{
			// Arrange
			var updatedChip = new Chip { ChipID = 1, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = false };
			_mockChipService.Setup(service => service.UpdateChipAsync(updatedChip))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _chipsController.UpdateChip(1, updatedChip);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteChip_ReturnsNotFound_WhenChipDoesNotExist()
		{
			// Arrange
			_mockChipService.Setup(service => service.GetChipByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Chip)null);

			// Act
			var result = await _chipsController.DeleteChip(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteChip_ReturnsNoContent_WhenChipIsDeleted()
		{
			// Arrange
			var existingChip = new Chip { ChipID = 1, MemberID = 1, IssueDate = System.DateTime.Now, IsActive = true };
			_mockChipService.Setup(service => service.GetChipByIdAsync(1))
				.ReturnsAsync(existingChip);
			_mockChipService.Setup(service => service.DeleteChipAsync(1))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _chipsController.DeleteChip(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
