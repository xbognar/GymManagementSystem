using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Controllers;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
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
			var member = new Member { MemberID = 10, FirstName = "John" };
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(10)).ReturnsAsync(member);

			// Act
			var result = await _controller.GetMember(10);

			// Assert
			var okResult = Assert.IsType<Member>(result.Value);
			okResult.MemberID.Should().Be(10);
		}

		/// <summary>
		/// Tests that GetMember returns NotFound if the member is not found.
		/// </summary>
		[Fact]
		public async Task GetMember_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(999)).ReturnsAsync((Member)null);

			// Act
			var result = await _controller.GetMember(999);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		/// <summary>
		/// Tests that GetAllMembers returns Ok with a list of members.
		/// </summary>
		[Fact]
		public async Task GetAllMembers_ReturnsOkWithList()
		{
			// Arrange
			var members = new List<Member> { new Member(), new Member() };
			_memberServiceMock.Setup(s => s.GetAllMembersAsync()).ReturnsAsync(members);

			// Act
			var result = await _controller.GetAllMembers();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedList = Assert.IsType<List<Member>>(okResult.Value);
			returnedList.Count.Should().Be(2);
		}

		/// <summary>
		/// Tests that AddMember returns CreatedAtAction with the newly created member.
		/// </summary>
		[Fact]
		public async Task AddMember_ReturnsCreatedAtAction()
		{
			// Arrange
			var newMember = new Member { MemberID = 3 };
			_memberServiceMock.Setup(s => s.AddMemberAsync(newMember)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.AddMember(newMember);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			createdResult.RouteValues["id"].Should().Be(3);
			createdResult.Value.Should().Be(newMember);
		}

		/// <summary>
		/// Tests that UpdateMember returns NoContent when the ID matches.
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMatches_ReturnsNoContent()
		{
			// Arrange
			var member = new Member { MemberID = 2 };
			_memberServiceMock.Setup(s => s.UpdateMemberAsync(member)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateMember(2, member);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that UpdateMember returns BadRequest if the route ID does not match the member's ID.
		/// </summary>
		[Fact]
		public async Task UpdateMember_WhenIdMismatch_ReturnsBadRequest()
		{
			// Arrange
			var member = new Member { MemberID = 2 };

			// Act
			var result = await _controller.UpdateMember(999, member);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		/// <summary>
		/// Tests that DeleteMember returns NoContent when deletion succeeds.
		/// </summary>
		[Fact]
		public async Task DeleteMember_WhenFound_ReturnsNoContent()
		{
			// Arrange
			var existing = new Member { MemberID = 5 };
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(5)).ReturnsAsync(existing);

			// Act
			var result = await _controller.DeleteMember(5);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		/// <summary>
		/// Tests that DeleteMember returns NotFound if the member does not exist.
		/// </summary>
		[Fact]
		public async Task DeleteMember_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_memberServiceMock.Setup(s => s.GetMemberByIdAsync(999)).ReturnsAsync((Member)null);

			// Act
			var result = await _controller.DeleteMember(999);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		/// <summary>
		/// Tests that GetMemberIdByName returns Ok with the member ID if found.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenMatchFound_ReturnsOk()
		{
			// Arrange
			_memberServiceMock.Setup(s => s.GetMemberIdByNameAsync("John Doe")).ReturnsAsync(10);

			// Act
			var result = await _controller.GetMemberIdByName("John Doe");

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			(okResult.Value as int?).Should().Be(10);
		}

		/// <summary>
		/// Tests that GetMemberIdByName returns NotFound if not found.
		/// </summary>
		[Fact]
		public async Task GetMemberIdByName_WhenNoMatch_ReturnsNotFound()
		{
			// Arrange
			_memberServiceMock.Setup(s => s.GetMemberIdByNameAsync("Unknown Person")).ReturnsAsync((int?)null);

			// Act
			var result = await _controller.GetMemberIdByName("Unknown Person");

			// Assert
			Assert.IsType<NotFoundObjectResult>(result.Result);
		}
	}
}
