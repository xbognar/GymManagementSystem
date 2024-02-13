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
	public class PaymentsControllerTests
	{
		private readonly Mock<IPaymentService> _mockPaymentService;
		private readonly PaymentsController _paymentsController;

		public PaymentsControllerTests()
		{
			_mockPaymentService = new Mock<IPaymentService>();
			_paymentsController = new PaymentsController(_mockPaymentService.Object);
		}

		[Fact]
		public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
		{
			// Arrange
			_mockPaymentService.Setup(service => service.GetPaymentByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Payment)null);

			// Act
			var result = await _paymentsController.GetPayment(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task GetPayment_ReturnsPayment_WhenPaymentExists()
		{
			// Arrange
			var expectedPayment = new Payment { PaymentID = 1, MemberID = 1, Amount = 100M, PaymentDate = System.DateTime.Now, PaymentMethod = "Cash" };
			_mockPaymentService.Setup(service => service.GetPaymentByIdAsync(1))
				.ReturnsAsync(expectedPayment);

			// Act
			var result = await _paymentsController.GetPayment(1);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedPayment = Assert.IsType<Payment>(okResult.Value);
			Assert.Equal(expectedPayment, returnedPayment);
		}

		[Fact]
		public async Task GetAllPayments_ReturnsAllPayments()
		{
			// Arrange
			var expectedPayments = new List<Payment>
			{
				new Payment { PaymentID = 1, MemberID = 1, Amount = 100M, PaymentDate = System.DateTime.Now, PaymentMethod = "Cash" },
				new Payment { PaymentID = 2, MemberID = 2, Amount = 200M, PaymentDate = System.DateTime.Now, PaymentMethod = "Credit Card" }
			};
			_mockPaymentService.Setup(service => service.GetAllPaymentsAsync())
				.ReturnsAsync(expectedPayments);

			// Act
			var result = await _paymentsController.GetAllPayments();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnedPayments = Assert.IsType<List<Payment>>(okResult.Value);
			Assert.Equal(expectedPayments, returnedPayments);
		}

		[Fact]
		public async Task AddPayment_ReturnsCreatedAtAction_WhenPaymentIsAdded()
		{
			// Arrange
			var newPayment = new Payment { PaymentID = 3, MemberID = 3, Amount = 300M, PaymentDate = System.DateTime.Now, PaymentMethod = "Debit Card" };
			_mockPaymentService.Setup(service => service.AddPaymentAsync(newPayment))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _paymentsController.AddPayment(newPayment);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			var returnedPayment = Assert.IsType<Payment>(createdAtActionResult.Value);
			Assert.Equal(newPayment, returnedPayment);
			Assert.Equal("GetPayment", createdAtActionResult.ActionName);
			Assert.Equal(newPayment.PaymentID, createdAtActionResult.RouteValues["id"]);
		}

		[Fact]
		public async Task UpdatePayment_ReturnsBadRequest_WhenPaymentIdDoesNotMatch()
		{
			// Arrange
			var updatedPayment = new Payment { PaymentID = 1, MemberID = 1, Amount = 150M, PaymentDate = System.DateTime.Now, PaymentMethod = "Online" };

			// Act
			var result = await _paymentsController.UpdatePayment(2, updatedPayment);

			// Assert
			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task UpdatePayment_ReturnsNoContent_WhenPaymentIsUpdated()
		{
			// Arrange
			var updatedPayment = new Payment { PaymentID = 1, MemberID = 1, Amount = 150M, PaymentDate = System.DateTime.Now, PaymentMethod = "Online" };
			_mockPaymentService.Setup(service => service.UpdatePaymentAsync(updatedPayment))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _paymentsController.UpdatePayment(1, updatedPayment);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeletePayment_ReturnsNotFound_WhenPaymentDoesNotExist()
		{
			// Arrange
			_mockPaymentService.Setup(service => service.GetPaymentByIdAsync(It.IsAny<int>()))
				.ReturnsAsync((Payment)null);

			// Act
			var result = await _paymentsController.DeletePayment(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeletePayment_ReturnsNoContent_WhenPaymentIsDeleted()
		{
			// Arrange
			var existingPayment = new Payment { PaymentID = 1, MemberID = 1, Amount = 100M, PaymentDate = System.DateTime.Now, PaymentMethod = "Cash" };
			_mockPaymentService.Setup(service => service.GetPaymentByIdAsync(1))
				.ReturnsAsync(existingPayment);
			_mockPaymentService.Setup(service => service.DeletePaymentAsync(1))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _paymentsController.DeletePayment(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}