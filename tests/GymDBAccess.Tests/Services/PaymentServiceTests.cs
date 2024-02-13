using Xunit;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class PaymentServiceTests : IDisposable
{
	private readonly ApplicationDbContext _context;
	private readonly PaymentService _paymentService;

	public PaymentServiceTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test run
			.Options;

		_context = new ApplicationDbContext(options);
		_paymentService = new PaymentService(_context);

		// Seed the database with some test payments
		_context.Payments.Add(new Payment { PaymentID = 1, MemberID = 1, Amount = 100.00M, PaymentDate = DateTime.Now.AddDays(-30), PaymentMethod = "Cash" });
		_context.Payments.Add(new Payment { PaymentID = 2, MemberID = 2, Amount = 200.00M, PaymentDate = DateTime.Now.AddDays(-15), PaymentMethod = "Credit Card" });
		_context.SaveChanges();
	}

	[Fact]
	public async Task GetPaymentByIdAsync_ReturnsPayment()
	{
		// Act
		var result = await _paymentService.GetPaymentByIdAsync(1);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(100.00M, result.Amount);
		Assert.Equal("Cash", result.PaymentMethod);
	}

	[Fact]
	public async Task GetAllPaymentsAsync_ReturnsAllPayments()
	{
		// Act
		var result = await _paymentService.GetAllPaymentsAsync();

		// Assert
		var payments = result.ToList();
		Assert.Equal(2, payments.Count);
	}

	[Fact]
	public async Task AddPaymentAsync_AddsPayment()
	{
		// Arrange
		var newPayment = new Payment { PaymentID = 3, MemberID = 3, Amount = 300.00M, PaymentDate = DateTime.Now, PaymentMethod = "Debit Card" };

		// Act
		await _paymentService.AddPaymentAsync(newPayment);
		var addedPayment = await _context.Payments.FindAsync(3);

		// Assert
		Assert.NotNull(addedPayment);
		Assert.Equal(300.00M, addedPayment.Amount);
		Assert.Equal("Debit Card", addedPayment.PaymentMethod);
	}

	[Fact]
	public async Task UpdatePaymentAsync_UpdatesPayment()
	{
		// Arrange
		var paymentToUpdate = await _context.Payments.FindAsync(1);
		paymentToUpdate.Amount = 150.00M;

		// Act
		await _paymentService.UpdatePaymentAsync(paymentToUpdate);
		var updatedPayment = await _context.Payments.FindAsync(1);

		// Assert
		Assert.NotNull(updatedPayment);
		Assert.Equal(150.00M, updatedPayment.Amount);
	}

	[Fact]
	public async Task DeletePaymentAsync_DeletesPayment()
	{
		// Act
		await _paymentService.DeletePaymentAsync(2);
		var deletedPayment = await _context.Payments.FindAsync(2);

		// Assert
		Assert.Null(deletedPayment);
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}
}
