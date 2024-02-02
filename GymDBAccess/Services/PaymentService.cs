using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymDBAccess.Services
{
    public class PaymentService : IPaymentService
	{
		private readonly ApplicationDbContext _context;

		public PaymentService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Payment> GetPaymentByIdAsync(int id)
		{
			return await _context.Payments.FindAsync(id);
		}

		public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
		{
			return await _context.Payments.ToListAsync();
		}

		public async Task AddPaymentAsync(Payment payment)
		{
			await _context.Payments.AddAsync(payment);
			await _context.SaveChangesAsync();
		}

		public async Task UpdatePaymentAsync(Payment payment)
		{
			_context.Payments.Update(payment);
			await _context.SaveChangesAsync();
		}

		public async Task DeletePaymentAsync(int id)
		{
			var paymentToDelete = await _context.Payments.FindAsync(id);
			if (paymentToDelete != null)
			{
				_context.Payments.Remove(paymentToDelete);
				await _context.SaveChangesAsync();
			}
		}
	}
}
