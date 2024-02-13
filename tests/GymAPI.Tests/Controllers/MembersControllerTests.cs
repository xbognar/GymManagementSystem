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
	public class MembersControllerTests
	{
		private readonly Mock<IMemberService> _mockMemberService;
		private readonly MembersController _membersController;

		public MembersControllerTests()
		{
			_mockMemberService = new Mock<IMemberService>();
			_membersController = new MembersController(_mockMemberService.Object);
		}

		[Fact]
		public async Task GetMember_ReturnsNotFound_WhenMemberDoesNotExist()
		{
			// Arrange
			_mockMemberService.Setup(service => service.GetMemberByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Member)null);

			// Act
			var result = await _membersController.GetMember(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task GetMember_ReturnsMember_WhenMemberExists()
		{
			// Arrange
			var expectedMember = new Member { MemberID = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890" };
			_mockMemberService.Setup(service => service.GetMemberByIdAsync(1))
				.ReturnsAsync(expectedMember);

			// Act
			var actionResult = await _membersController.GetMember(1);

			// Assert
			var okResult = Assert.IsType<ActionResult<Member>>(actionResult);
			var actualMember = Assert.IsType<Member>(okResult.Value);
			Assert.Equal(expectedMember.MemberID, actualMember.MemberID);
			Assert.Equal(expectedMember.FirstName, actualMember.FirstName);
		}

		[Fact]
		public async Task GetAllMembers_ReturnsAllMembers()
		{
			// Arrange
			var expectedMembers = new List<Member>
			{
				new Member { MemberID = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890" },
				new Member { MemberID = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", PhoneNumber = "0987654321" }
			};
			_mockMemberService.Setup(service => service.GetAllMembersAsync())
				.ReturnsAsync(expectedMembers);

			// Act
			var result = await _membersController.GetAllMembers();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedMembers = Assert.IsType<List<Member>>(okResult.Value);
			Assert.Equal(expectedMembers, returnedMembers);
		}

		[Fact]
		public async Task AddMember_ReturnsCreatedAtAction_WhenMemberIsAdded()
		{
			// Arrange
			var newMember = new Member { MemberID = 3, FirstName = "Jim", LastName = "Bean", Email = "jim@example.com", PhoneNumber = "1231231234" };
			_mockMemberService.Setup(service => service.AddMemberAsync(newMember))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membersController.AddMember(newMember);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedMember = Assert.IsType<Member>(createdAtActionResult.Value);
			Assert.Equal(newMember, returnedMember);
			Assert.Equal("GetMember", createdAtActionResult.ActionName);
		}

		[Fact]
		public async Task UpdateMember_ReturnsBadRequest_WhenMemberIdDoesNotMatch()
		{
			// Arrange
			var updatedMember = new Member { MemberID = 1, FirstName = "Updated", LastName = "Name", Email = "updated@example.com", PhoneNumber = "1231231234" };

			// Act
			var result = await _membersController.UpdateMember(2, updatedMember);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task UpdateMember_ReturnsNoContent_WhenMemberIsUpdated()
		{
			// Arrange
			var updatedMember = new Member { MemberID = 1, FirstName = "Updated", LastName = "Name", Email = "updated@example.com", PhoneNumber = "1231231234" };
			_mockMemberService.Setup(service => service.UpdateMemberAsync(updatedMember))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membersController.UpdateMember(1, updatedMember);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteMember_ReturnsNotFound_WhenMemberDoesNotExist()
		{
			// Arrange
			_mockMemberService.Setup(service => service.GetMemberByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Member)null);

			// Act
			var result = await _membersController.DeleteMember(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteMember_ReturnsNoContent_WhenMemberIsDeleted()
		{
			// Arrange
			var existingMember = new Member { MemberID = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890" };
			_mockMemberService.Setup(service => service.GetMemberByIdAsync(1))
				.ReturnsAsync(existingMember);
			_mockMemberService.Setup(service => service.DeleteMemberAsync(1))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _membersController.DeleteMember(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}
