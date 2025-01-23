using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GymAPI.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
	/// <summary>
	/// Unit tests for the MembersController.
	/// </summary>
	public class MembersControllerTests
	{
		private readonly Mock<IMemberService> _memberServiceMock;
		private readonly MembersController _controller;

		public MembersControllerTests()
		{
			_memberServiceMock = new Mock<IMemberService>();
			_controller = new MembersController(_memberServiceMock.Object);
		}

		/// <summary>
		/// Tests that GetMember returns Ok with the member if found.
		/// </summary>
		[Fact]
		public async Task GetMember_WhenFound_ReturnsOk()
		{
			// Arrange
			var memberId = 10;
			var member = new Member { MemberID = memberId, FirstName = "John", LastName = "Doe" };
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(memberId))
							  .ReturnsAsync(member);

			// Act
			var result = await _controller.GetMember(memberId);

			// Assert
			var okResult = result.Value as Member;
			okResult.Should().NotBeNull();
			okResult.MemberID.Should().Be(memberId);
			okResult.FirstName.Should().Be("John");
			okResult.LastName.Should().Be("Doe");
		}

		/// <summary>
		/// Tests that GetMember returns NotFound if the member is not found.
		/// </summary>
		[Fact]
		public async Task GetMember_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var memberId = 999;
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(memberId))
							  .ReturnsAsync((Member)null);

			// Act
			var result = await _controller.GetMember(memberId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that GetAllMembers returns Ok with a list of members.
		/// </summary>
		[Fact]
		public async Task GetAllMembers_ReturnsOkWithList()
		{
			// Arrange
			var members = new List<Member>
			{
				new Member { MemberID = 1, FirstName = "Alice", LastName = "Smith" },
				new Member { MemberID = 2, FirstName = "Bob", LastName = "Johnson" }
			};
			_memberServiceMock.Setup(s => s.GetAllMembersAsync())
							  .ReturnsAsync(members);

			// Act
			var result = await _controller.GetAllMembers();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);

			var returnedMembers = okResult.Value as List<Member>;
			returnedMembers.Should().NotBeNull();
			returnedMembers.Count.Should().Be(2);
			returnedMembers.Should().Contain(m => m.MemberID == 1);
			returnedMembers.Should().Contain(m => m.MemberID == 2);
		}

		/// <summary>
		/// Tests that AddMember returns CreatedAtAction with the newly created member.
		/// </summary>
		[Fact]
		public async Task AddMember_ReturnsCreatedAtAction()
		{
			// Arrange
			var newMember = new Member { FirstName = "Testy", LastName = "McTestFace" };
			var createdMember = new Member { MemberID = 3, FirstName = "Testy", LastName = "McTestFace" };

			_memberServiceMock.Setup(s => s.AddMemberAsync(It.IsAny<Member>()))
							  .Callback<Member>(m => m.MemberID = 3)
							  .Returns(Task.CompletedTask);
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(3))
							  .ReturnsAsync(createdMember);

			// Act
			var result = await _controller.AddMember(newMember);

			// Assert
			var createdAtActionResult = result.Result as CreatedAtActionResult;
			createdAtActionResult.Should().NotBeNull();
			createdAtActionResult.StatusCode.Should().Be(201);
			createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetMember));
			createdAtActionResult.RouteValues["id"].Should().Be(3);
			createdAtActionResult.Value.Should().BeEquivalentTo(createdMember);
		}

		/// <summary>
		/// Tests that UpdateMember returns NoContent when the update is successful.
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var memberId = 2;
			var updatedMember = new Member { MemberID = memberId, FirstName = "Bob", LastName = "Updated" };

			_memberServiceMock.Setup(s => s.UpdateMemberAsync(updatedMember))
							  .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateMember(memberId, updatedMember);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that UpdateMember returns BadRequest when the route ID does not match the member's ID.
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var routeId = 999;
			var updatedMember = new Member { MemberID = 2, FirstName = "Bob", LastName = "Mismatch" };

			// Act
			var result = await _controller.UpdateMember(routeId, updatedMember);

			// Assert
			result.Should().BeOfType<BadRequestResult>();
		}

		/// <summary>
		/// Tests that DeleteMember returns NoContent when the member is successfully deleted.
		/// </summary>
		[Fact]
		public async Task DeleteMember_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var memberId = 5;
			var existingMember = new Member { MemberID = memberId, FirstName = "Charlie", LastName = "Brown" };

			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(memberId))
							  .ReturnsAsync(existingMember);
			_memberServiceMock.Setup(s => s.DeleteMemberAsync(memberId))
							  .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteMember(memberId);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		/// <summary>
		/// Tests that DeleteMember returns NotFound when the member does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteMember_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			var memberId = 999;
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(memberId))
							  .ReturnsAsync((Member)null);

			// Act
			var result = await _controller.DeleteMember(memberId);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
		}

		/// <summary>
		/// Tests that GetMemberIdByName returns Ok with the member ID when a match is found.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenMatchFound_ReturnsOk()
		{
			// Arrange
			var fullName = "John Doe";
			var memberId = 10;
			_memberServiceMock.Setup(s => s.GetMemberIdByNameAsync(fullName))
							  .ReturnsAsync(memberId);

			// Act
			var result = await _controller.GetMemberIdByName(fullName);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(200);
			okResult.Value.Should().Be(memberId);
		}

		/// <summary>
		/// Tests that GetMemberIdByName returns NotFound when no match is found.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenNoMatch_ReturnsNotFound()
		{
			// Arrange
			var fullName = "Unknown Person";
			_memberServiceMock.Setup(s => s.GetMemberIdByNameAsync(fullName))
							  .ReturnsAsync((int?)null);

			// Act
			var result = await _controller.GetMemberIdByName(fullName);

			// Assert
			var notFoundResult = result.Result as NotFoundObjectResult;
			notFoundResult.Should().NotBeNull();
			notFoundResult.StatusCode.Should().Be(404);
			notFoundResult.Value.Should().Be("Member not found or full name format is incorrect.");
		}
	}
}
