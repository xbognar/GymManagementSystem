using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymDBAccess.DTOs;

namespace UnitTests.Controllers
{
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
		/// Tests that GetChip returns Ok if the chip is found.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenFound_ReturnsOk()
		{
			// Arrange
			var chip = new Chip { ChipID = 10 };
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(10)).ReturnsAsync(chip);

			// Act
			var result = await _controller.GetChip(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedChip = Assert.IsType<Chip>(okResult.Value);
			returnedChip.ChipID.Should().Be(10);
		}

		/// <summary>
		/// Tests that GetChip returns NotFound if the chip does not exist.
		/// </summary>
		[Fact]
		public async Task GetChip_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(999)).ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.GetChip(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that GetAllChips returns Ok with a list of chips.
		/// </summary>
		[Fact]
		public async Task GetAllChips_ReturnsOkWithList()
		{
			// Arrange
			var chips = new List<Chip> { new Chip { ChipID = 1 }, new Chip { ChipID = 2 } };
			_chipServiceMock.Setup(s => s.GetAllChipsAsync()).ReturnsAsync(chips);

			// Act
			var result = await _controller.GetAllChips();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<Chip>>(okResult.Value);
			returnedList.Count.Should().Be(2);
		}

		/// <summary>
		/// Tests that AddChip returns CreatedAtAction with the newly created chip.
		/// </summary>
		[Fact]
		public async Task AddChip_ReturnsCreatedAtAction()
		{
			// Arrange
			var newChip = new Chip { ChipID = 5 };
			_chipServiceMock.Setup(s => s.AddChipAsync(newChip)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.AddChip(newChip);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newChip);
		}

		/// <summary>
		/// Tests that UpdateChip returns BadRequest if route ID and request.ChipID do not match.
		/// </summary>
		[Fact]
		public async Task UpdateChip_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var updateReq = new ChipUpdateRequest
			{
				ChipID = 10,
				NewMemberID = 123
			};

			// Act
			var result = await _controller.UpdateChip(999, updateReq);

			// Assert
			var badRequest = Assert.IsType<BadRequestObjectResult>(result);
			badRequest.Value.Should().Be("Chip ID does not match the route ID.");
		}

		/// <summary>
		/// Tests that UpdateChip returns NotFound if no chip is found by the given ID.
		/// </summary>
		[Fact]
		public async Task UpdateChip_WhenChipNotFound_ReturnsNotFound()
		{
			// Arrange
			var updateReq = new ChipUpdateRequest
			{
				ChipID = 10,
				NewMemberID = 123
			};

			_chipServiceMock.Setup(s => s.GetChipByIdAsync(10)).ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.UpdateChip(10, updateReq);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		/// <summary>
		/// Tests that UpdateChip returns Ok with the updated chip if successful.
		/// </summary>
		[Fact]
		public async Task UpdateChip_SuccessfulUpdate_ReturnsOk()
		{
			// Arrange
			var existingChip = new Chip { ChipID = 10, MemberID = 50 };
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(10)).ReturnsAsync(existingChip);

			var updateReq = new ChipUpdateRequest
			{
				ChipID = 10,
				NewMemberID = 123
			};

			// Act
			var result = await _controller.UpdateChip(10, updateReq);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var updatedChip = Assert.IsType<Chip>(okResult.Value);
			updatedChip.MemberID.Should().Be(123);
		}

		/// <summary>
		/// Tests that DeleteChip returns NotFound if the chip is not found.
		/// </summary>
		[Fact]
		public async Task DeleteChip_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(999)).ReturnsAsync((Chip)null);

			// Act
			var result = await _controller.DeleteChip(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		/// <summary>
		/// Tests that DeleteChip returns NoContent when deletion succeeds.
		/// </summary>
		[Fact]
		public async Task DeleteChip_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var chip = new Chip { ChipID = 10 };
			_chipServiceMock.Setup(s => s.GetChipByIdAsync(10)).ReturnsAsync(chip);

			// Act
			var result = await _controller.DeleteChip(10);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that GetActiveChips returns Ok with a list of active chip DTOs.
		/// </summary>
		[Fact]
		public async Task GetActiveChips_ReturnsOkWithActiveDTOList()
		{
			// Arrange
			var active = new List<ActiveChipDTO>
			{
				new ActiveChipDTO { ChipID = 1, OwnerFullName = "Alice Smith", ChipInfo = "Chip Info" }
			};
			_chipServiceMock.Setup(s => s.GetActiveChipsAsync()).ReturnsAsync(active);

			// Act
			var result = await _controller.GetActiveChips();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var list = Assert.IsType<List<ActiveChipDTO>>(okResult.Value);
			list.Should().HaveCount(1);
		}

		/// <summary>
		/// Tests that GetInactiveChips returns Ok with a list of inactive chip DTOs.
		/// </summary>
		[Fact]
		public async Task GetInactiveChips_ReturnsOkWithInactiveDTOList()
		{
			// Arrange
			var inactive = new List<InactiveChipDTO>
			{
				new InactiveChipDTO { ChipID = 2, OwnerFullName = "Bob Jones", ChipInfo = "Some Info" }
			};
			_chipServiceMock.Setup(s => s.GetInactiveChipsAsync()).ReturnsAsync(inactive);

			// Act
			var result = await _controller.GetInactiveChips();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var list = Assert.IsType<List<InactiveChipDTO>>(okResult.Value);
			list.Should().HaveCount(1);
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberId returns NotFound if no chip info is found.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenNull_ReturnsNotFound()
		{
			// Arrange
			_chipServiceMock.Setup(s => s.GetChipInfoByMemberIdAsync(999)).ReturnsAsync((string)null);

			// Act
			var result = await _controller.GetChipInfoByMemberId(999);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result.Result);
		}

		/// <summary>
		/// Tests that GetChipInfoByMemberId returns Ok with the chip info if found.
		/// </summary>
		[Fact]
		public async Task GetChipInfoByMemberId_WhenFound_ReturnsOk()
		{
			// Arrange
			_chipServiceMock.Setup(s => s.GetChipInfoByMemberIdAsync(100)).ReturnsAsync("VIP Access");

			// Act
			var result = await _controller.GetChipInfoByMemberId(100);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			okResult.Value.Should().Be("VIP Access");
		}
	}
}
