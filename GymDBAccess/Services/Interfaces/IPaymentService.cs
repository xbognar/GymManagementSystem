using GymDBAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymDBAccess.Services.Interfaces
{
	public interface IPaymentService
	{
		
		Task<Payment> GetPaymentByIdAsync(int id);
		
		Task<IEnumerable<Payment>> GetAllPaymentsAsync();
		
		Task AddPaymentAsync(Payment payment);
		
		Task UpdatePaymentAsync(Payment payment);
		
		Task DeletePaymentAsync(int id);
	
	}
}
