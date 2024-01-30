using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.Models
{
	public class Membership
	{

		public int MembershipID { get; set; }
		
		public int MemberID { get; set; }
		
		public DateTime StartDate { get; set; }
		
		public DateTime EndDate { get; set; }
		
		public string Type { get; set; }

	}
}
