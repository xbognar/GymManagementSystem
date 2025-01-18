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
using System.Linq;

namespace UnitTests.Controllers
{
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
		/// Tests that GetMembership returns Ok if membership is found.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenFound_ReturnsOk()
		{
			// Arrange
			var membership = new Membership { MembershipID = 10 };
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(10)).ReturnsAsync(membership);

			// Act
			var result = await _controller.GetMembership(10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returned = Assert.IsType<Membership>(okResult.Value);
			returned.MembershipID.Should().Be(10);
		}

		/// <summary>
		/// Tests that GetMembership returns NotFound if membership is not found.
		/// </summary>
		[Fact]
		public async Task GetMembership_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(999)).ReturnsAsync((Membership)null);

			// Act
			var result = await _controller.GetMembership(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that GetAllMemberships returns Ok with a list of memberships.
		/// </summary>
		[Fact]
		public async Task GetAllMemberships_ReturnsOkWithList()
		{
			// Arrange
			var memberships = new List<Membership> { new Membership(), new Membership() };
			_membershipServiceMock.Setup(s => s.GetAllMembershipsAsync()).ReturnsAsync(memberships);

			// Act
			var result = await _controller.GetAllMemberships();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var list = Assert.IsType<List<Membership>>(okResult.Value);
			list.Count.Should().Be(2);
		}

		/// <summary>
		/// Tests that AddMembership returns CreatedAtAction for the new membership.
		/// </summary>
		[Fact]
		public async Task AddMembership_ReturnsCreatedAtAction()
		{
			// Arrange
			var newMembership = new Membership { MembershipID = 5 };
			_membershipServiceMock.Setup(s => s.AddMembershipAsync(newMembership)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.AddMembership(newMembership);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			createdResult.RouteValues["id"].Should().Be(5);
			createdResult.Value.Should().Be(newMembership);
		}

		/// <summary>
		/// Tests that UpdateMembership returns NoContent.
		/// (Membership ID mismatch is not specifically checked in the controller code.)
		/// </summary>
		[Fact]
		public async Task UpdateMembership_ReturnsNoContent()
		{
			// Arrange
			var membership = new Membership { MembershipID = 2 };
			_membershipServiceMock.Setup(s => s.UpdateMembershipAsync(membership)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateMembership(2, membership);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that DeleteMembership returns NotFound if membership is not found.
		/// </summary>
		[Fact]
		public async Task DeleteMembership_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(999)).ReturnsAsync((Membership)null);

			// Act
			var result = await _controller.DeleteMembership(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		/// <summary>
		/// Tests that DeleteMembership returns NoContent if found and removed.
		/// </summary>
		[Fact]
		public async Task DeleteMembership_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var membership = new Membership { MembershipID = 10 };
			_membershipServiceMock.Setup(s => s.GetMembershipByIdAsync(10)).ReturnsAsync(membership);

			// Act
			var result = await _controller.DeleteMembership(10);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that GetActiveMemberships returns Ok with a list of active membership DTOs.
		/// </summary>
		[Fact]
		public async Task GetActiveMemberships_ReturnsOkWithActiveList()
		{
			// Arrange
			var activeList = new List<ActiveMembershipDTO>
			{
				new ActiveMembershipDTO { MembershipID = 1, MemberName = "Alice Smith" }
			};
			_membershipServiceMock.Setup(s => s.GetActiveMembershipsAsync()).ReturnsAsync(activeList);

			// Act
			var result = await _controller.GetActiveMemberships();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var list = Assert.IsType<List<ActiveMembershipDTO>>(okResult.Value);
			list.Count.Should().Be(1);
		}

		/// <summary>
		/// Tests that GetInactiveMemberships returns Ok with a list of inactive membership DTOs.
		/// </summary>
		[Fact]
		public async Task GetInactiveMemberships_ReturnsOkWithInactiveList()
		{
			// Arrange
			var inactiveList = new List<InactiveMembershipDTO>
			{
				new InactiveMembershipDTO { MembershipID = 2, MemberName = "John Doe" }
			};
			_membershipServiceMock.Setup(s => s.GetInactiveMembershipsAsync()).ReturnsAsync(inactiveList);

			// Act
			var result = await _controller.GetInactiveMemberships();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var list = Assert.IsType<List<InactiveMembershipDTO>>(okResult.Value);
			list.Count.Should().Be(1);
		}

		/// <summary>
		/// Tests that GetUserMemberships returns NotFound if no memberships are returned.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenNone_ReturnsNotFound()
		{
			// Arrange
			_membershipServiceMock.Setup(s => s.GetUserMembershipsAsync(999)).ReturnsAsync(new List<UserMembershipsDTO>());

			// Act
			var result = await _controller.GetUserMemberships(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that GetUserMemberships returns Ok with the user's memberships if found.
		/// </summary>
		[Fact]
		public async Task GetUserMemberships_WhenFound_ReturnsOk()
		{
			// Arrange
			var userMemberships = new List<UserMembershipsDTO>
			{
				new UserMembershipsDTO { MembershipID = 100, PaymentType = "Monthly" }
			};
			_membershipServiceMock.Setup(s => s.GetUserMembershipsAsync(5)).ReturnsAsync(userMemberships);

			// Act
			var result = await _controller.GetUserMemberships(5);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<UserMembershipsDTO>>(okResult.Value);
			returnedList.First().MembershipID.Should().Be(100);
		}
	}
}
