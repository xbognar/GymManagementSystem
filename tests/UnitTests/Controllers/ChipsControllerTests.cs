using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GymAPI.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymDBAccess.DTOs;

namespace UnitTests.Controllers
{
	/// <summary>
	/// Unit tests for the ChipsController.
	/// </summary>
	public class ChipsControllerTests
	{
		private readonly Mock<IChipService> _chipServiceMock;
		private readonly ChipsController _controller;

		public ChipsControllerTests()
		{
			_chipServiceMock = new Mock<IChipService>();
			_controller = new ChipsController(_chipServiceMock.Object);
		}

		/// <summary>
		/// Tests that GetChip returns Ok with the chip if found.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenFound_ReturnsOk()
		{
			// Arrange
			var chipId = 10;
			var chip = new Chip { ChipID = chipId, MemberID = 100, ChipInfo = "VIP Access", IsActive = true };
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync(chip);

			// Act
			var result = await _controller.GetChip(chipId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedChip = okResult.Value as Chip;
			returnedChip.Should().NotBeNull();
			returnedChip.ChipID.Should().Be(chipId);
			returnedChip.ChipInfo.Should().Be("VIP Access");
		}

		/// <summary>
		/// Tests that GetChip returns NotFound if the chip does not exist.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var chipId = 999;
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.GetChip(chipId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that GetAllChips returns Ok with a list of chips.
		/// </summary>
		[Fact]
		public async Task GetAllChips_ReturnsOkWithList()
		{
			// Arrange
			var chips = new List<Chip>
			{
				new Chip { ChipID = 1, MemberID = 101, ChipInfo = "VIP Access", IsActive = true },
				new Chip { ChipID = 2, MemberID = 102, ChipInfo = "Basic Access", IsActive = false }
			};
			_chipServiceMock.Setup(s => s.GetAllChipsAsync())
						   .ReturnsAsync(chips);

			// Act
			var result = await _controller.GetAllChips();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedChips = okResult.Value as List<Chip>;
			returnedChips.Should().NotBeNull();
			returnedChips.Count.Should().Be(2);
			returnedChips.Should().Contain(c => c.ChipID == 1);
			returnedChips.Should().Contain(c => c.ChipID == 2);
		}

		/// <summary>
		/// Tests that AddChip returns CreatedAtAction with the newly created chip.
		/// </summary>
		[Fact]
		public async Task AddChip_ReturnsCreatedAtAction()
		{
			// Arrange
			var newChip = new Chip { MemberID = 103, ChipInfo = "Standard Access", IsActive = true };
			var createdChip = new Chip { ChipID = 5, MemberID = 103, ChipInfo = "Standard Access", IsActive = true };

			_chipServiceMock.Setup(s => s.AddChipAsync(It.IsAny<Chip>()))
						   .Callback<Chip>(c => c.ChipID = 5)
						   .Returns(Task.CompletedTask);
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(5))
						   .ReturnsAsync(createdChip);

			// Act
			var result = await _controller.AddChip(newChip);

			// Assert
			var createdAtActionResult = result.Result as CreatedAtActionResult;
			createdAtActionResult.Should().NotBeNull();
			createdAtActionResult.StatusCode.Should().Be(201);
			createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetChip));
			createdAtActionResult.RouteValues["id"].Should().Be(5);
			createdAtActionResult.Value.Should().BeEquivalentTo(createdChip);
		}

		/// <summary>
		/// Tests that UpdateChip returns BadRequest when the route ID does not match the request's ChipID.
		/// </summary>
		[Fact]
		public async Task UpdateChip_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var routeId = 999;
			var updateRequest = new ChipUpdateRequest
			{
				ChipID = 10,
				NewMemberID = 123
			};

			// Act
			var result = await _controller.UpdateChip(routeId, updateRequest);

			// Assert
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult.Should().NotBeNull();
			badRequestResult.StatusCode.Should().Be(400);
			badRequestResult.Value.Should().Be("Chip ID does not match the route ID.");
		}

		/// <summary>
		/// Tests that UpdateChip returns NotFound when the chip to update does not exist.
		/// </summary>
		[Fact]
		public async Task UpdateChip_WhenChipNotFound_ReturnsNotFound()
		{
			// Arrange
			var chipId = 10;
			var updateRequest = new ChipUpdateRequest
			{
				ChipID = chipId,
				NewMemberID = 123
			};

			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.UpdateChip(chipId, updateRequest);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that UpdateChip returns Ok with the updated chip when the update is successful.
		/// </summary>
		[Fact]
		public async Task UpdateChip_SuccessfulUpdate_ReturnsOk()
		{
			// Arrange
			var chipId = 10;
			var existingChip = new Chip { ChipID = chipId, MemberID = 50, ChipInfo = "Standard Access", IsActive = true };
			var updateRequest = new ChipUpdateRequest
			{
				ChipID = chipId,
				NewMemberID = 123
			};
			var updatedChip = new Chip { ChipID = chipId, MemberID = 123, ChipInfo = "Standard Access", IsActive = true };

			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync(existingChip);
			_chipServiceMock.Setup(s => s.UpdateChipAsync(existingChip))
						   .Returns(Task.CompletedTask);
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync(updatedChip);

			// Act
			var result = await _controller.UpdateChip(chipId, updateRequest);

			// Assert
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);
			okResult.Value.Should().BeEquivalentTo(updatedChip);
		}

		/// <summary>
		/// Tests that DeleteChip returns NotFound when the chip does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteChip_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var chipId = 999;
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.DeleteChip(chipId);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that DeleteChip returns NoContent when the chip is successfully deleted.
		/// </summary>
		[Fact]
		public async Task DeleteChip_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var chipId = 10;
			var existingChip = new Chip { ChipID = chipId, MemberID = 50, ChipInfo = "Standard Access", IsActive = true };

			_chipServiceMock.Setup(s => s.GetChipByIdAsync(chipId))
						   .ReturnsAsync(existingChip);
			_chipServiceMock.Setup(s => s.DeleteChipAsync(chipId))
						   .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteChip(chipId);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that GetActiveChips returns Ok with a list of active chips.
		/// </summary>
		[Fact]
		public async Task GetActiveChips_ReturnsOkWithActiveList()
		{
			// Arrange
			var activeChips = new List<ActiveChipDTO>
			{
				new ActiveChipDTO { ChipID = 1, OwnerFullName = "Alice Smith", ChipInfo = "VIP Access" }
			};
			_chipServiceMock.Setup(s => s.GetActiveChipsAsync())
						   .ReturnsAsync(activeChips);

			// Act
			var result = await _controller.GetActiveChips();

			// Assert
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedChips = okResult.Value as List<ActiveChipDTO>;
			returnedChips.Should().NotBeNull();
			returnedChips.Count.Should().Be(1);
			returnedChips[0].ChipID.Should().Be(1);
			returnedChips[0].OwnerFullName.Should().Be("Alice Smith");
			returnedChips[0].ChipInfo.Should().Be("VIP Access");
		}

		/// <summary>
		/// Tests that GetInactiveChips returns Ok with a list of inactive chips.
		/// </summary>
		[Fact]
		public async Task GetInactiveChips_ReturnsOkWithInactiveList()
		{
			// Arrange
			var inactiveChips = new List<InactiveChipDTO>
			{
				new InactiveChipDTO { ChipID = 2, OwnerFullName = "Bob Jones", ChipInfo = "Basic Access" }
			};
			_chipServiceMock.Setup(s => s.GetInactiveChipsAsync())
						   .ReturnsAsync(inactiveChips);

			// Act
			var result = await _controller.GetInactiveChips();

			// Assert
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedChips = okResult.Value as List<InactiveChipDTO>;
			returnedChips.Should().NotBeNull();
			returnedChips.Count.Should().Be(1);
			returnedChips[0].ChipID.Should().Be(2);
			returnedChips[0].OwnerFullName.Should().Be("Bob Jones");
			returnedChips[0].ChipInfo.Should().Be("Basic Access");
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberId returns NotFound when no chip info is found.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenNull_ReturnsNotFound()
		{
			// Arrange
			var memberId = 999;
			_chipServiceMock.Setup(s => s.GetChipInfoByMemberIdAsync(memberId))
						   .ReturnsAsync((string)null);

			// Act
			var result = await _controller.GetChipInfoByMemberId(memberId);

			// Assert
			var notFoundResult = result.Result as NotFoundObjectResult;
			notFoundResult.Should().NotBeNull();
			notFoundResult.StatusCode.Should().Be(404);
			notFoundResult.Value.Should().Be($"No chip found for member with ID {memberId}.");
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberId returns Ok with chip info when found.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenFound_ReturnsOk()
		{
			// Arrange
			var memberId = 100;
			var chipInfo = "VIP Access";
			_chipServiceMock.Setup(s => s.GetChipInfoByMemberIdAsync(memberId))
						   .ReturnsAsync(chipInfo);

			// Act
			var result = await _controller.GetChipInfoByMemberId(memberId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);
			okResult.Value.Should().Be(chipInfo);
		}
	}
}
