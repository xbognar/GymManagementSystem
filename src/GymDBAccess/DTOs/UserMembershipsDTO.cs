using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.DTOs
{
	public class UserMembershipsDTO
	{
		public int MembershipID { get; set; }
		
		public DateTime StartDate { get; set; }
		
		public DateTime EndDate { get; set; }
		
		public string? PaymentType { get; set; }
	
	}

}
