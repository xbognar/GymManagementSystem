using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GymDBAccess.Controllers;
using GymDBAccess.Models;
using GymDBAccess.DTOs;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymAPI.Tests.Controllers
{
	public class MembershipsControllerTests
	{
		private readonly Mock<IMembershipService> _mockMembershipService;
		private readonly MembershipsController _membershipsController;

		public MembershipsControllerTests()
		{
			_mockMembershipService = new Mock<IMembershipService>();
			_membershipsController = new MembershipsController(_mockMembershipService.Object);
		}

		[Fact]
		public async Task GetMembership_ReturnsNotFound_WhenMembershipDoesNotExist()
		{
			// Arrange
			_mockMembershipService.Setup(service => service.GetMembershipByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Membership)null);

			// Act
			var result = await _membershipsController.GetMembership(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task GetMembership_ReturnsMembership_WhenMembershipExists()
		{
			// Arrange
			var expectedMembership = new Membership { MembershipID = 1, MemberID = 1, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddYears(1), PaymentType = "Annual", IsActive = true };
			_mockMembershipService.Setup(service => service.GetMembershipByIdAsync(1))
				.ReturnsAsync(expectedMembership);

			// Act
			var result = await _membershipsController.GetMembership(1);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedMembership = Assert.IsType<Membership>(okResult.Value);
			Assert.Equal(expectedMembership, returnedMembership);
		}

		[Fact]
		public async Task AddMembership_ReturnsCreatedAtAction_WhenMembershipIsAdded()
		{
			// Arrange
			var newMembership = new Membership { MembershipID = 3, MemberID = 3, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddYears(1), PaymentType = "Monthly", IsActive = true };
			_mockMembershipService.Setup(service => service.AddMembershipAsync(newMembership))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membershipsController.AddMembership(newMembership);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedMembership = Assert.IsType<Membership>(createdAtActionResult.Value);
			Assert.Equal(newMembership, returnedMembership);
			Assert.Equal("GetMembership", createdAtActionResult.ActionName);
		}

		[Fact]
		public async Task UpdateMembership_ReturnsBadRequest_WhenMembershipIdDoesNotMatch()
		{
			// Arrange
			var updatedMembership = new Membership { MembershipID = 1, MemberID = 1, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddYears(1), PaymentType = "Monthly", IsActive = false };

			// Act
			var result = await _membershipsController.UpdateMembership(2, updatedMembership);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task UpdateMembership_ReturnsNoContent_WhenMembershipIsUpdated()
		{
			// Arrange
			var updatedMembership = new Membership { MembershipID = 1, MemberID = 1, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddYears(1), PaymentType = "Monthly", IsActive = false };
			_mockMembershipService.Setup(service => service.UpdateMembershipAsync(updatedMembership))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membershipsController.UpdateMembership(1, updatedMembership);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteMembership_ReturnsNotFound_WhenMembershipDoesNotExist()
		{
			// Arrange
			_mockMembershipService.Setup(service => service.GetMembershipByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Membership)null);

			// Act
			var result = await _membershipsController.DeleteMembership(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteMembership_ReturnsNoContent_WhenMembershipIsDeleted()
		{
			// Arrange
			var existingMembership = new Membership { MembershipID = 1, MemberID = 1, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddYears(1), PaymentType = "Annual", IsActive = true };
			_mockMembershipService.Setup(service => service.GetMembershipByIdAsync(1))
				.ReturnsAsync(existingMembership);
			_mockMembershipService.Setup(service => service.DeleteMembershipAsnyc(1))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membershipsController.DeleteMembership(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task GetActiveMemberships_ReturnsActiveMemberships()
		{
			// Arrange
			var expectedMemberships = new List<ActiveMembershipDTO>
			{
				new ActiveMembershipDTO
				{
					MembershipID = 1,
					MemberName = "John Doe",
					StartDate = System.DateTime.Now.AddMonths(-1),
					EndDate = System.DateTime.Now.AddMonths(11),
					Type = "Annual"
				}
			};
			_mockMembershipService.Setup(service => service.GetActiveMembershipsAsync())
				.ReturnsAsync(expectedMemberships);

			// Act
			var result = await _membershipsController.GetActiveMemberships();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedMemberships = Assert.IsType<List<ActiveMembershipDTO>>(okResult.Value);
			Assert.Equal(expectedMemberships, returnedMemberships);
		}

		[Fact]
		public async Task GetInactiveMemberships_ReturnsInactiveMemberships()
		{
			// Arrange
			var expectedMemberships = new List<InactiveMembershipDTO>
			{
				new InactiveMembershipDTO
				{
					MembershipID = 2,
					MemberName = "Jane Doe",
					StartDate = System.DateTime.Now.AddMonths(-12),
					EndDate = System.DateTime.Now.AddMonths(-2),
					Type = "Annual"
				}
			};
			_mockMembershipService.Setup(service => service.GetInactiveMembershipsAsync())
				.ReturnsAsync(expectedMemberships);

			// Act
			var result = await _membershipsController.GetInactiveMemberships();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedMemberships = Assert.IsType<List<InactiveMembershipDTO>>(okResult.Value);
			Assert.Equal(expectedMemberships, returnedMemberships);
		}

	}
}
