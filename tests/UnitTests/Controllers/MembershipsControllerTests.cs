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
	/// Unit tests for the MembershipsController.
	/// </summary>
	public class MembershipsControllerTests
	{
		private readonly Mock<IMembershipService> _membershipServiceMock;
		private readonly MembershipsController _controller;

		public MembershipsControllerTests()
		{
			_membershipServiceMock = new Mock<IMembershipService>();
			_controller = new MembershipsController(_membershipServiceMock.Object);
		}

		/// <summary>
		/// Tests that GetMembership returns Ok with the membership if found.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenFound_ReturnsOk()
		{
			// Arrange
			var membershipId = 10;
			var membership = new Membership { MembershipID = membershipId, MemberID = 100, PaymentType = "Monthly", IsActive = true };
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(membershipId))
								   .ReturnsAsync(membership);

			// Act
			var result = await _controller.GetMembership(membershipId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedMembership = okResult.Value as Membership;
			returnedMembership.Should().NotBeNull();
			returnedMembership.MembershipID.Should().Be(membershipId);
			returnedMembership.PaymentType.Should().Be("Monthly");
		}

		/// <summary>
		/// Tests that GetMembership returns NotFound if the membership does not exist.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var membershipId = 999;
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(membershipId))
								   .ReturnsAsync((Membership)null);

			// Act
			var result = await _controller.GetMembership(membershipId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that GetAllMemberships returns Ok with a list of memberships.
		/// </summary>
		[Fact]
		public async Task GetAllMemberships_ReturnsOkWithList()
		{
			// Arrange
			var memberships = new List<Membership>
			{
				new Membership { MembershipID = 1, MemberID = 101, PaymentType = "Monthly", IsActive = true },
				new Membership { MembershipID = 2, MemberID = 102, PaymentType = "Annual", IsActive = false }
			};
			_membershipServiceMock.Setup(s => s.GetAllMembershipsAsync())
								   .ReturnsAsync(memberships);

			// Act
			var result = await _controller.GetAllMemberships();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedMemberships = okResult.Value as List<Membership>;
			returnedMemberships.Should().NotBeNull();
			returnedMemberships.Count.Should().Be(2);
			returnedMemberships.Should().Contain(m => m.MembershipID == 1);
			returnedMemberships.Should().Contain(m => m.MembershipID == 2);
		}

		/// <summary>
		/// Tests that AddMembership returns CreatedAtAction with the newly created membership.
		/// </summary>
		[Fact]
		public async Task AddMembership_ReturnsCreatedAtAction()
		{
			// Arrange
			var newMembership = new Membership { MemberID = 103, PaymentType = "Weekly", IsActive = true };
			var createdMembership = new Membership { MembershipID = 5, MemberID = 103, PaymentType = "Weekly", IsActive = true };

			_membershipServiceMock.Setup(s => s.AddMembershipAsync(It.IsAny<Membership>()))
								   .Callback<Membership>(m => m.MembershipID = 5)
								   .Returns(Task.CompletedTask);
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(5))
								   .ReturnsAsync(createdMembership);

			// Act
			var result = await _controller.AddMembership(newMembership);

			// Assert
			var createdAtActionResult = result.Result as CreatedAtActionResult;
			createdAtActionResult.Should().NotBeNull();
			createdAtActionResult.StatusCode.Should().Be(201);
			createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetMembership));
			createdAtActionResult.RouteValues.Should().NotBeNull();
			createdAtActionResult.Value.Should().BeEquivalentTo(createdMembership);
		}

		/// <summary>
		/// Tests that UpdateMembership returns NoContent when the update is successful.
		/// </summary>
		[Fact]
		public async Task UpdateMembership_ReturnsNoContent()
		{
			// Arrange
			var membershipId = 2;
			var updatedMembership = new Membership { MembershipID = membershipId, MemberID = 101, PaymentType = "UpdatedType", IsActive = true };

			_membershipServiceMock.Setup(s => s.UpdateMembershipAsync(updatedMembership))
								   .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateMembership(membershipId, updatedMembership);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that DeleteMembership returns NotFound when the membership does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteMembership_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var membershipId = 999;
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(membershipId))
								   .ReturnsAsync((Membership)null);

			// Act
			var result = await _controller.DeleteMembership(membershipId);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that DeleteMembership returns NoContent when the membership is successfully deleted.
		/// </summary>
		[Fact]
		public async Task DeleteMembership_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var membershipId = 10;
			var existingMembership = new Membership { MembershipID = membershipId, MemberID = 104, PaymentType = "Monthly", IsActive = true };

			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(membershipId))
								   .ReturnsAsync(existingMembership);
			_membershipServiceMock.Setup(s => s.DeleteMembershipAsnyc(membershipId)) // Fixed method name
								   .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteMembership(membershipId);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that GetActiveMemberships returns Ok with a list of active memberships.
		/// </summary>
		[Fact]
		public async Task GetActiveMemberships_ReturnsOkWithActiveList()
		{
			// Arrange
			var activeMemberships = new List<ActiveMembershipDTO>
			{
				new ActiveMembershipDTO { MembershipID = 1, MemberName = "Alice Smith", StartDate = new System.DateTime(2023, 1, 1), EndDate = new System.DateTime(2023, 12, 31), Type = "Monthly" }
			};
			_membershipServiceMock.Setup(s => s.GetActiveMembershipsAsync())
								   .ReturnsAsync(activeMemberships);

			// Act
			var result = await _controller.GetActiveMemberships();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedList = okResult.Value as List<ActiveMembershipDTO>;
			returnedList.Should().NotBeNull();
			returnedList.Count.Should().Be(1);
			returnedList[0].MembershipID.Should().Be(1);
			returnedList[0].MemberName.Should().Be("Alice Smith");
			returnedList[0].Type.Should().Be("Monthly");
		}

		/// <summary>
		/// Tests that GetInactiveMemberships returns Ok with a list of inactive memberships.
		/// </summary>
		[Fact]
		public async Task GetInactiveMemberships_ReturnsOkWithInactiveList()
		{
			// Arrange
			var inactiveMemberships = new List<InactiveMembershipDTO>
			{
				new InactiveMembershipDTO { MembershipID = 2, MemberName = "John Doe", StartDate = new System.DateTime(2022, 1, 1), EndDate = new System.DateTime(2022, 12, 31), Type = "Annual" }
			};
			_membershipServiceMock.Setup(s => s.GetInactiveMembershipsAsync())
								   .ReturnsAsync(inactiveMemberships);

			// Act
			var result = await _controller.GetInactiveMemberships();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedList = okResult.Value as List<InactiveMembershipDTO>;
			returnedList.Should().NotBeNull();
			returnedList.Count.Should().Be(1);
			returnedList[0].MembershipID.Should().Be(2);
			returnedList[0].MemberName.Should().Be("John Doe");
			returnedList[0].Type.Should().Be("Annual");
		}

		/// <summary>
		/// Tests that GetUserMemberships returns NotFound when no memberships are found for the user.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenNone_ReturnsNotFound()
		{
			// Arrange
			var userId = 999;
			_membershipServiceMock.Setup(s => s.GetUserMembershipsAsync(userId))
								   .ReturnsAsync(new List<UserMembershipsDTO>());

			// Act
			var result = await _controller.GetUserMemberships(userId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that GetUserMemberships returns Ok with the user's memberships when found.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenFound_ReturnsOk()
		{
			// Arrange
			var userId = 5;
			var userMemberships = new List<UserMembershipsDTO>
			{
				new UserMembershipsDTO { MembershipID = 100, StartDate = new System.DateTime(2023, 1, 1), EndDate = new System.DateTime(2023, 6, 30), PaymentType = "Monthly" }
			};
			_membershipServiceMock.Setup(s => s.GetUserMembershipsAsync(userId))
								   .ReturnsAsync(userMemberships);

			// Act
			var result = await _controller.GetUserMemberships(userId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedList = okResult.Value as List<UserMembershipsDTO>;
			returnedList.Should().NotBeNull();
			returnedList.Count.Should().Be(1);
			returnedList[0].MembershipID.Should().Be(100);
			returnedList[0].PaymentType.Should().Be("Monthly");
		}
	}
}
