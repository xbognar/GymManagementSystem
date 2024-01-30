using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.Models
{
	public class Chip
	{

		public int ChipID { get; set; }
		
		public int MemberID { get; set; }
		
		public DateTime IssueDate { get; set; }
		
		public bool IsActive { get; set; }

	}
}
