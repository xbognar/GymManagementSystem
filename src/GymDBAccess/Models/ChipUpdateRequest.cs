using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.Models
{
	public class ChipUpdateRequest
	{
		public int ChipID { get; set; }
		
		public int NewMemberID { get; set; }
	}
}
