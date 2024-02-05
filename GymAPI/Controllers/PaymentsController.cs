using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GymDBAccess.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentsController : ControllerBase
	{
		private readonly IPaymentService _paymentService;

		public PaymentsController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		// GET: api/Payments/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Payment>> GetPayment(int id)
		{
			var payment = await _paymentService.GetPaymentByIdAsync(id);
			if (payment == null)
			{
				return NotFound();
			}
			return Ok(payment);
		}

		// GET: api/Payments
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
		{
			var payments = await _paymentService.GetAllPaymentsAsync();
			return Ok(payments);
		}

		// POST: api/Payments
		[HttpPost]
		public async Task<ActionResult<Payment>> AddPayment([FromBody] Payment payment)
		{
			await _paymentService.AddPaymentAsync(payment);
			return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentID }, payment);
		}

		// PUT: api/Payments/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
		{
			if (id != payment.PaymentID)
			{
				return BadRequest();
			}

			await _paymentService.UpdatePaymentAsync(payment);
			return NoContent();
		}

		// DELETE: api/Payments/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePayment(int id)
		{
			var payment = await _paymentService.GetPaymentByIdAsync(id);
			if (payment == null)
			{
				return NotFound();
			}

			await _paymentService.DeletePaymentAsync(id);
			return NoContent();
		}
	}
}
